using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game2DWaterKit
{
	[
		RequireComponent ( typeof( MeshRenderer ) ),
		RequireComponent ( typeof( MeshFilter ) ),
		RequireComponent ( typeof( BoxCollider2D ) ),
		RequireComponent ( typeof( BuoyancyEffector2D ) )
	]

	[ExecuteInEditMode]

	public class Game2DWater : MonoBehaviour
	{
		#region variables

		[SerializeField] Vector2 waterSize = Vector2.one;
		[SerializeField] Vector2 lastWaterSize = Vector2.one;
		[SerializeField] int subdivisionsCountPerUnit = 3;

		[SerializeField] float minimumDisturbance = 0.1f;
		[SerializeField] float maximumDisturbance = 0.75f;
		[SerializeField] float velocityMultiplier = 0.12f;
		[SerializeField] LayerMask collisionMask = ~( 1 << 4 );
		[SerializeField] float collisionMinimumDepth = -10f;
		[SerializeField] float collisionMaximumDepth = 10f;

		[SerializeField] float damping = 0.05f;
		[SerializeField] float stiffness = 60f;
		[SerializeField] float stiffnessSquareRoot = Mathf.Sqrt ( 60f );
		[SerializeField] float spread = 60f;
		[SerializeField] bool useCustomBoundaries = false;
		[SerializeField] float firstCustomBoundary = 0.5f;
		[SerializeField] float secondCustomBoundary = -0.5f;
		[SerializeField] float lastSecondCustomBoundary = -0.5f;
		[SerializeField] float lastFirstCustomBoundary = 0.5f;
		[SerializeField] float buoyancyEffectorSurfaceLevel = 0.02f;

		[SerializeField] float refractionRenderTextureResizeFactor = 1f;
		[SerializeField] LayerMask refractionCullingMask = ~( 1 << 4 );
		[SerializeField] float reflectionRenderTextureResizeFactor = 1f;
		[SerializeField] LayerMask reflectionCullingMask = ~( 1 << 4 );
		[SerializeField] float reflectionZOffset = 0f;

		[SerializeField] int sortingLayerID = 0;
		[SerializeField] int sortingOrder = 0;
		[SerializeField] float farClipPlane = 100f;
		[SerializeField] bool renderPixelLights = true;
		[SerializeField] bool allowMSAA = false;
		[SerializeField] bool allowHDR = false;

		[SerializeField] AudioClip splashAudioClip = null;
		[SerializeField] float minimumAudioPitch = 0.75f;
		[SerializeField] float maximumAudioPitch = 1.25f;

		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		Mesh mesh;
		Material waterMaterial;
		BoxCollider2D boxCollider;
		BuoyancyEffector2D buoyancyEffector;
		AudioSource audioSource;
		EdgeCollider2D edgeCollider;
		Vector2[] edgeColliderPoints = new Vector2[4];

		Camera waterCamera;
		RenderTexture refractionRenderTexture;
		RenderTexture reflectionRenderTexture;
		bool renderRefraction;
		bool renderReflection;
		bool updateWaterRenderSettings;
		int refractionTextureID;
		int reflectionTextureID;
		int waterMatrixID;
		Vector3 lastBoundsScreenSpaceMin = Vector3.zero;
		Vector3 lastBoundsScreenSpaceMax = Vector3.zero;
		Vector3 waterCameraPositionForReflectionRendering = Vector3.zero;
		Vector3 waterCameraPositionForRefractionRendering = Vector3.zero;

		int surfaceVerticesCount;
		List<Vector3> vertices;
		List<Vector2> uvs;
		List<int> triangles;
		bool updateWaterSimulation;
		float waterPositionOfRest;
		float[] velocities;

		#endregion

		#region Properties

		/// <summary>
		/// Sets the water size. X represents the width and Y represents the height.
		/// </summary>
		public Vector2 WaterSize {
			get {
				return waterSize;
			}
			set {
				value.x = Mathf.Clamp ( value.x, 0f, float.MaxValue );
				value.y = Mathf.Clamp ( value.y, 0f, float.MaxValue );
				if ( waterSize == value )
					return;
				waterSize = value;
				RecomputeMesh ();
			}
		}

		/// <summary>
		/// Sets the number of water’s surface vertices within one unit.
		/// </summary>
		public int SubdivisionsCountPerUnit {
			get {
				return subdivisionsCountPerUnit;
			}
			set {
				value = Mathf.Clamp ( value, 0, int.MaxValue );
				if ( subdivisionsCountPerUnit == value )
					return;
				subdivisionsCountPerUnit = value;
				RecomputeMesh ();
			}
		}

		/// <summary>
		/// Enable/Disable using custom wave boundaries. When waves reach a boundary, they bounce back.
		/// </summary>
		public bool UseCustomBoundaries {
			get {
				return useCustomBoundaries;
			}
			set {
				if ( useCustomBoundaries == value )
					return;
				useCustomBoundaries = value;
				RecomputeMesh ();
			}
		}

		/// <summary>
		/// The location of the first boundary.
		/// </summary>
		public float FirstCustomBoundary {
			get {
				return firstCustomBoundary;
			}
			set {
				float halfWidth = waterSize.x / 2f;
				value = Mathf.Clamp ( value, -halfWidth, halfWidth );
				if ( Mathf.Approximately ( firstCustomBoundary, value ) )
					return;
				firstCustomBoundary = value;
				if ( useCustomBoundaries )
					RecomputeMesh ();
			}
		}

		/// <summary>
		/// The location of the second boundary.
		/// </summary>
		public float SecondCustomBoundary {
			get {
				return secondCustomBoundary;
			}
			set {
				float halfWidth = waterSize.x / 2f;
				value = Mathf.Clamp ( value, -halfWidth, halfWidth );
				if ( Mathf.Approximately ( secondCustomBoundary, value ) )
					return;
				secondCustomBoundary = value;
				if ( useCustomBoundaries )
					RecomputeMesh ();
			}
		}

		/// <summary>
		/// Sets the surface location of the buoyancy fluid. When a GameObject is above this line, no buoyancy forces are applied. When a GameObject is intersecting or completely below this line, buoyancy forces are applied.
		/// </summary>
		public float BuoyancyEffectorSurfaceLevel {
			get {
				return buoyancyEffectorSurfaceLevel;
			}
			set {
				value = Mathf.Clamp01 ( value );
				if ( Mathf.Approximately ( buoyancyEffectorSurfaceLevel, value ) )
					return;
				buoyancyEffectorSurfaceLevel = value;
				if ( buoyancyEffector )
					buoyancyEffector.surfaceLevel = waterSize.y * ( 0.5f - buoyancyEffectorSurfaceLevel );
			}
		}

		/// <summary>
		/// The minimum displacement of water’s surface when an GameObject falls into water.
		/// </summary>
		public float MinimumDisturbance {
			get {
				return minimumDisturbance;
			}
			set {
				minimumDisturbance = Mathf.Clamp ( value, 0f, float.MaxValue );
			}
		}

		/// <summary>
		/// The maximum displacement of water’s surface when an GameObject falls into water.
		/// </summary>
		public float MaximumDisturbance {
			get {
				return maximumDisturbance;
			}
			set {
				maximumDisturbance = Mathf.Clamp ( value, 0f, float.MaxValue );
			}
		}

		/// <summary>
		/// When a rigidbody falls into water, the amount of water’s surface displacement is determined by multiplying the rigidbody velocity by this factor.
		/// </summary>
		public float VelocityMultiplier {
			get {
				return velocityMultiplier;
			}
			set {
				velocityMultiplier = Mathf.Clamp ( value, 0f, float.MaxValue );
			}
		}

		/// <summary>
		/// Controls how fast the waves decay. A low value will make waves oscillate for a long time, while a high value will make waves oscillate for a short time.
		/// </summary>
		public float Damping {
			get {
				return damping;
			}
			set {
				damping = Mathf.Clamp01 ( value );
			}
		}

		/// <summary>
		/// Controls how fast the waves spread.
		/// </summary>
		public float Spread {
			get {
				return spread;
			}
			set {
				spread = Mathf.Clamp ( value, 0f, float.MaxValue );
			}
		}

		/// <summary>
		/// Controls the frequency of wave vibration. A low value will make waves oscillate slowly, while a high value will make waves oscillate quickly.
		/// </summary>
		public float Stiffness {
			get {
				return stiffness;
			}
			set {
				stiffness = Mathf.Clamp ( value, 0f, float.MaxValue );
				stiffnessSquareRoot = Mathf.Sqrt ( stiffness );
			}
		}

		/// <summary>
		/// Only GameObjects on these layers will disturb the water’s surface (produce waves) when they fall into water.
		/// </summary>
		public LayerMask CollisionMask {
			get {
				return collisionMask;
			}
			set {
				collisionMask = value & ~( 1 << 4 );
			}
		}

		/// <summary>
		/// Only GameObjects with Z coordinate (depth) greater than or equal to this value will disturb the water’s surface when they fall into water.
		/// </summary>
		public float MinimumCollisionDepth {
			get {
				return collisionMinimumDepth;
			}
			set {
				collisionMinimumDepth = value;
			}
		}

		/// <summary>
		/// Only GameObjects with Z coordinate (depth) less than or equal to this value will disturb the water’s surface when they fall into water.
		/// </summary>
		public float MaximumCollisionDepth {
			get {
				return collisionMaximumDepth;
			}
			set {
				collisionMaximumDepth = value;
			}
		}

		/// <summary>
		/// Specifies how much the RenderTexture used to render refraction is resized. Decreasing this value lowers the RenderTexture resolution and thus improves performance at the expense of visual quality.
		/// </summary>
		public float RefractionRenderTextureResizeFactor {
			get {
				return refractionRenderTextureResizeFactor;
			}
			set {
				value = Mathf.Clamp01 ( value );
				if ( Mathf.Approximately ( refractionRenderTextureResizeFactor, value ) )
					return;
				refractionRenderTextureResizeFactor = value;
				updateWaterRenderSettings = true;
			}
		}

		/// <summary>
		/// Only GameObjects on these layers will be rendered.
		/// </summary>
		public LayerMask RefractionCullingMask {
			get {
				return refractionCullingMask;
			}
			set {
				refractionCullingMask = value & ~( 1 << 4 );
			}
		}

		/// <summary>
		/// Specifies how much the RenderTexture used to render reflection is resized. Decreasing this value lowers the RenderTexture resolution and thus improves performance at the expense of visual quality.
		/// </summary>
		public float ReflectionRenderTextureResizeFactor {
			get {
				return reflectionRenderTextureResizeFactor;
			}
			set {
				value = Mathf.Clamp01 ( value );
				if ( Mathf.Approximately ( reflectionRenderTextureResizeFactor, value ) )
					return;
				reflectionRenderTextureResizeFactor = value;
				updateWaterRenderSettings = true;
			}
		}

		/// <summary>
		/// Only GameObjects on these layers will be rendered.
		/// </summary>
		public LayerMask ReflectionCullingMask {
			get {
				return reflectionCullingMask;
			}
			set {
				reflectionCullingMask = value & ~( 1 << 4 );
			}
		}

		/// <summary>
		/// Only GameObjects on these layers will be rendered.
		/// </summary>
		public float ReflectionZOffset {
			get {
				return reflectionZOffset;
			}
			set {
				reflectionZOffset = value;
				waterCameraPositionForReflectionRendering.z = transform.position.z + reflectionZOffset;
			}
		}

		/// <summary>
		/// The name of the water mesh renderer sorting layer.
		/// </summary>
		public int SortingLayerID {
			get {
				return sortingLayerID;
			}
			set {
				
				if ( sortingLayerID == value )
					return;
				sortingLayerID = value;
				if ( meshRenderer )
					meshRenderer.sortingLayerID = sortingLayerID;
			}
		}

		/// <summary>
		/// The water mesh renderer order within a sorting layer.
		/// </summary>
		public int SortingOrder {
			get {
				return sortingOrder;
			}
			set {
				if ( sortingOrder == value )
					return;
				sortingOrder = value;
				if ( meshRenderer )
					meshRenderer.sortingOrder = sortingOrder;
			}
		}

		/// <summary>
		/// Controls whether the rendered objects will be affected by pixel lights. Disabling this could increase performance at the expense of visual fidelity.
		/// </summary>
		public bool RenderPixelLights {
			get {
				return renderPixelLights;
			}
			set {
				renderPixelLights = value;
			}
		}

		/// <summary>
		/// Sets the furthest point relative to the water that will be drawn when rendering refraction and/or reflection.
		/// </summary>
		public float FarClipPlane {
			get {
				return farClipPlane;
			}
			set {
				if ( Mathf.Approximately ( farClipPlane, value ) )
					return;
				farClipPlane = value;
				updateWaterRenderSettings = true;
				if ( waterCamera )
					waterCamera.farClipPlane = farClipPlane;
			}
		}

		/// <summary>
		/// Allow multisample antialiasing rendering.
		/// </summary>
		public bool AllowMSAA {
			get {
				return allowMSAA;
			}
			set {
				if ( allowMSAA == value )
					return;
				allowMSAA = value;
				if ( waterCamera )
					waterCamera.allowMSAA = allowMSAA;
			}
		}

		/// <summary>
		/// Allow high dynamic range rendering.
		/// </summary>
		public bool AllowHDR {
			get {
				return allowHDR;
			}
			set {
				allowHDR = value;
				if ( waterCamera )
					waterCamera.allowHDR = allowHDR;
			}
		}

		/// <summary>
		/// The AudioClip asset to play when a GameObject falls into water.
		/// </summary>
		public AudioClip SplashAudioClip {
			get {
				return splashAudioClip;
			}
			set {
				splashAudioClip = value;
			}
		}

		/// <summary>
		/// Sets the minimum frequency of the splash clip.
		/// </summary>
		public float MinimumAudioPitch {
			get {
				return minimumAudioPitch;
			}
			set {
				minimumAudioPitch = Mathf.Clamp ( value, -3f, 3f );
			}
		}

		/// <summary>
		/// Sets the maximum frequency of the splash clip.
		/// </summary>
		public float MaximumAudioPitch {
			get {
				return maximumAudioPitch;
			}
			set {
				maximumAudioPitch = Mathf.Clamp ( value, -3f, 3f );
			}
		}

		#endregion

		#region Methods

		#if UNITY_EDITOR
		// Add menu item to create Game2D Water GameObject.
		// Priority 10 ensures it is grouped with the other menu items of the same kind and propagated to the hierarchy dropdown and hierarchy context menus.
		[ MenuItem ( "GameObject/2D Object/Game2D Water", false, 10 )]
		static void CreateCustomGameObject ( MenuCommand menuCommand )
		{
			GameObject go = new GameObject ( "Game2D Water" );
			go.AddComponent <Game2DWater> ();
			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign ( go, menuCommand.context as GameObject );
			Undo.RegisterCreatedObjectUndo ( go, "Create" + go.name );
			Selection.activeObject = go;
		}

		//Reset is called when the user hits the Reset button in the Inspector's context menu
		void Reset ()
		{
			if ( waterCamera ) {
				DestroyImmediate ( waterCamera.gameObject );
				waterCamera = null;
			}

			waterSize = Vector2.one;
			lastWaterSize = Vector2.one;
			subdivisionsCountPerUnit = 3;

			minimumDisturbance = 0.1f;
			maximumDisturbance = 0.75f;
			velocityMultiplier = 0.12f;
			collisionMask = ~( 1 << 4 );
			collisionMinimumDepth = -10f;
			collisionMaximumDepth = 10f;

			damping = 0.05f;
			stiffness = 60f;
			stiffnessSquareRoot = Mathf.Sqrt ( 60f );
			spread = 60f;
			useCustomBoundaries = false;
			firstCustomBoundary = 0.5f;
			secondCustomBoundary = -0.5f;
			lastSecondCustomBoundary = -0.5f;
			lastFirstCustomBoundary = 0.5f;
			buoyancyEffectorSurfaceLevel = 0.02f;

			refractionRenderTextureResizeFactor = 1f;
			refractionCullingMask = ~( 1 << 4 );
			reflectionRenderTextureResizeFactor = 1f;
			reflectionCullingMask = ~( 1 << 4 );
			reflectionZOffset = 0f;

			sortingLayerID = 0;
			sortingOrder = 0;
			farClipPlane = 100f;
			renderPixelLights = true;
			allowMSAA = false;
			allowHDR = false;

			splashAudioClip = null;
			minimumAudioPitch = 0.75f;
			maximumAudioPitch = 1.25f;

			RecomputeMesh ();
		}

		//This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
		public void OnValidate ()
		{
			refractionRenderTextureResizeFactor = Mathf.Clamp01 ( refractionRenderTextureResizeFactor );
			reflectionRenderTextureResizeFactor = Mathf.Clamp01 ( reflectionRenderTextureResizeFactor );
			reflectionCullingMask &= ~( 1 << 4 );
			refractionCullingMask &= ~( 1 << 4 );
			minimumDisturbance = Mathf.Clamp ( minimumDisturbance, 0f, float.MaxValue );
			maximumDisturbance = Mathf.Clamp ( maximumDisturbance, 0f, float.MaxValue );
			velocityMultiplier = Mathf.Clamp ( velocityMultiplier, 0f, float.MaxValue );
			damping = Mathf.Clamp01 ( damping );
			stiffness = Mathf.Clamp ( stiffness, 0f, float.MaxValue );
			stiffnessSquareRoot = Mathf.Sqrt ( stiffness );
			spread = Mathf.Clamp ( spread, 0f, float.MaxValue );
			buoyancyEffectorSurfaceLevel = Mathf.Clamp01 ( buoyancyEffectorSurfaceLevel );
			collisionMask &= ~( 1 << 4 );
			waterSize.x = Mathf.Clamp ( waterSize.x, 0f, float.MaxValue );
			waterSize.y = Mathf.Clamp ( waterSize.y, 0f, float.MaxValue );
			subdivisionsCountPerUnit = Mathf.Clamp ( subdivisionsCountPerUnit, 0, int.MaxValue );
			float halfWidth = waterSize.x / 2f;
			firstCustomBoundary = Mathf.Clamp ( firstCustomBoundary, -halfWidth, halfWidth );
			secondCustomBoundary = Mathf.Clamp ( secondCustomBoundary, -halfWidth, halfWidth );
			if ( meshFilter && meshFilter.sharedMesh ) {
				Vector3 meshSize = meshFilter.sharedMesh.bounds.size;
				int meshVerticesCount = meshFilter.sharedMesh.vertexCount;
				int vertexCount;
				if ( useCustomBoundaries ) {
					vertexCount = ( Mathf.RoundToInt ( Mathf.Abs ( secondCustomBoundary - firstCustomBoundary ) * subdivisionsCountPerUnit ) + 2 ) * 2;
				} else {
					vertexCount = ( Mathf.RoundToInt ( waterSize.x * subdivisionsCountPerUnit ) + 2 ) * 2;
				}
				if ( meshVerticesCount != vertexCount || !Mathf.Approximately ( meshSize.x, waterSize.x ) || !Mathf.Approximately ( meshSize.y, waterSize.y ) )
					RecomputeMesh ();
			}
			if ( waterCamera ) {
				waterCamera.allowMSAA = allowMSAA;
				waterCamera.allowHDR = allowHDR;
				waterCamera.farClipPlane = farClipPlane;
			}
			edgeCollider = GetComponent <EdgeCollider2D> ();
			if ( meshRenderer ) {
				meshRenderer.sortingLayerID = sortingLayerID;
				meshRenderer.sortingOrder = sortingOrder;
			}
		}

		#endif

		//This function is called when the behaviour becomes disabled () or inactive.
		void OnDisable ()
		{
			if ( waterCamera ) {
				DestroyImmediate ( waterCamera.gameObject );
				waterCamera = null;
			}
		}

		//Awake is called when the script instance is being loaded.
		void Awake ()
		{
			meshFilter = GetComponent <MeshFilter> ();
			meshRenderer = GetComponent <MeshRenderer> ();

			if ( !meshRenderer.sharedMaterial ) {
				waterMaterial = new Material ( Shader.Find ( "Game2DWaterKit/Unlit (Supports Lightmap)" ) );
			} else {
				//Ensure that when duplicating our gameobject, the duplicate copy gets its own unique material
				waterMaterial = new Material ( meshRenderer.sharedMaterial );
			}
			meshRenderer.sharedMaterial = waterMaterial;
			waterMaterial.name = "Game2DWater Material";
			meshRenderer.sharedMaterial = waterMaterial;
			meshRenderer.sortingLayerID = sortingLayerID;
			meshRenderer.sortingOrder = sortingOrder;

			boxCollider = GetComponent <BoxCollider2D> ();
			//BuoyancyEffector only works when an attached collider is marked as a trigger and used by effector 
			boxCollider.isTrigger = true;
			boxCollider.usedByEffector = true;

			edgeCollider = GetComponent <EdgeCollider2D> ();
			buoyancyEffector = GetComponent <BuoyancyEffector2D> ();
			audioSource = GetComponent <AudioSource> ();

			gameObject.layer = LayerMask.NameToLayer ( "Water" );

			renderRefraction = waterMaterial.IsKeywordEnabled ( "Water2D_Refraction" );
			renderReflection = waterMaterial.IsKeywordEnabled ( "Water2D_Reflection" );

			refractionTextureID = Shader.PropertyToID ( "_RefractionTexture" );
			reflectionTextureID = Shader.PropertyToID ( "_ReflectionTexture" );
			waterMatrixID = Shader.PropertyToID ( "_WaterMVP" );

			RecomputeMesh ();
		}

		//Sent when another object enters a trigger collider attached to this object (2D physics only).
		void OnTriggerEnter2D ( Collider2D other )
		{
			const float raycastDistance = 0.5f;
			Vector2 raycastDirection = Vector2.up;

			Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;

			float totalVelocities = 0f;
			int velocitiesCount = 0;

			int vertexIndex;
			int endIndex;
			int startIndex;
			if ( useCustomBoundaries ) {
				vertexIndex = 2;
				endIndex = surfaceVerticesCount - 1;
				startIndex = 1;
			} else {
				vertexIndex = 0;
				endIndex = surfaceVerticesCount;
				startIndex = 0;
			}

			for ( int i = startIndex ; i < endIndex ; i++,vertexIndex += 2 ) {
				Vector3 position = localToWorldMatrix.MultiplyPoint3x4 ( vertices [ vertexIndex ] );
				RaycastHit2D hit = Physics2D.Raycast ( position, raycastDirection, raycastDistance, collisionMask, collisionMinimumDepth, collisionMaximumDepth );
				if ( hit.rigidbody && hit.collider == other ) {
					float velocity = -hit.rigidbody.GetPointVelocity ( position ).y * velocityMultiplier;
					velocity = Mathf.Clamp ( velocity, minimumDisturbance, maximumDisturbance ) * stiffnessSquareRoot;
					totalVelocities += velocity;
					velocitiesCount++;
					updateWaterSimulation = true;
					velocities [ i ] -= velocity;
				}
			}

			if ( velocitiesCount > 0 ) {
				float meanVelocity = totalVelocities / velocitiesCount;
				PlaySplashSound ( meanVelocity );
			}
		}

		//This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
		void FixedUpdate ()
		{
			if ( updateWaterSimulation ) {
				updateWaterSimulation = false;

				float deltaTime = Time.fixedDeltaTime;
				float dampingFactor = damping * 2f * stiffnessSquareRoot;
				float spreadFactor = spread * subdivisionsCountPerUnit;

				int vertexIndex;
				int startIndex;
				int endIndex;

				if ( useCustomBoundaries ) {
					vertexIndex = 2;
					startIndex = 1;
					endIndex = surfaceVerticesCount - 1;
				} else {
					vertexIndex = 0;
					startIndex = 0;
					endIndex = surfaceVerticesCount;
				}

				Vector3 currentVertexPosition = vertices [ vertexIndex ];
				Vector3 prevVertexPosition = currentVertexPosition;
				Vector3 nextVertexPosition;

				for ( int i = startIndex ; i < endIndex ; i++,vertexIndex += 2 ) {
					nextVertexPosition = i < endIndex - 1 ? vertices [ vertexIndex + 2 ] : currentVertexPosition;

					float velocity = velocities [ i ];
					float restoringForce = stiffness * ( waterPositionOfRest - currentVertexPosition.y );
					float dampingForce = -dampingFactor * velocity;
					float spreadForce = spreadFactor * ( prevVertexPosition.y - currentVertexPosition.y + nextVertexPosition.y - currentVertexPosition.y );

					prevVertexPosition = currentVertexPosition;

					velocity += ( restoringForce + dampingForce + spreadForce ) * deltaTime;
					currentVertexPosition.y += velocity * deltaTime;

					vertices [ vertexIndex ] = currentVertexPosition;
					velocities [ i ] = velocity;

					currentVertexPosition = nextVertexPosition;

					if ( !updateWaterSimulation ) {
						//if all the velocities are in the [-0.0001,0.0001] range then, we don't need to continue updating the 
						//water simulation and thus improving performance
						const float UpdateWaterSimulationMinimumThreshold = 0.0001f;
						updateWaterSimulation |= velocity > UpdateWaterSimulationMinimumThreshold || velocity < -UpdateWaterSimulationMinimumThreshold;
					}
				}

				//Ensure that we apply the changes to the mesh
				mesh.SetVertices ( vertices );
				mesh.UploadMeshData ( false );
				mesh.RecalculateBounds ();
			}
		}

		//Update is called every frame, if the MonoBehaviour is enabled.
		void Update ()
		{
			//Useful when animating the water size or/and when animating the custom boundaries positions
			if ( waterSize != lastWaterSize
			     || ( useCustomBoundaries
			     && ( !Mathf.Approximately ( firstCustomBoundary, lastFirstCustomBoundary )
			     || !Mathf.Approximately ( secondCustomBoundary, lastSecondCustomBoundary ) ) ) ) {
				float halfWidth = waterSize.x / 2f;
				float halfDeltaHeight = ( waterSize.y - lastWaterSize.y ) / 2f;
				float xStep;
				float leftmostBoundary;

				if ( useCustomBoundaries ) {
					xStep = Mathf.Abs ( firstCustomBoundary - secondCustomBoundary ) / ( surfaceVerticesCount - 3 );
					leftmostBoundary = Mathf.Min ( firstCustomBoundary, secondCustomBoundary );
				
					lastFirstCustomBoundary = firstCustomBoundary;
					lastSecondCustomBoundary = secondCustomBoundary;
				} else {
					xStep = waterSize.x / ( surfaceVerticesCount - 1 );
					leftmostBoundary = -waterSize.x / 2f + xStep;
				}

				Vector3 vertexTop = vertices [ 0 ];
				Vector3 vertexBottom = vertices [ 1 ];

				vertexTop.x = vertexBottom.x = -halfWidth;
				vertexTop.y += halfDeltaHeight;
				vertexBottom.y -= halfDeltaHeight;
				vertices [ 0 ] = vertexTop;
				vertices [ 1 ] = vertexBottom;

				float xPosition = 0f;
				for ( int i = 1, vertexIndex = 2 ; i < surfaceVerticesCount - 1 ; i++ ,vertexIndex += 2 ) {
					vertexTop = vertices [ vertexIndex ];
					vertexBottom = vertices [ vertexIndex + 1 ];

					float x = xPosition + leftmostBoundary;
					xPosition += xStep;

					vertexTop.x = vertexBottom.x = x;
					vertexTop.y += halfDeltaHeight;
					vertexBottom.y -= halfDeltaHeight;
					vertices [ vertexIndex ] = vertexTop;
					vertices [ vertexIndex + 1 ] = vertexBottom;
				}

				int lastVertexIndex = ( surfaceVerticesCount - 1 ) * 2;
				vertexTop = vertices [ lastVertexIndex ];
				vertexBottom = vertices [ lastVertexIndex + 1 ];

				vertexTop.x = vertexBottom.x = halfWidth;
				vertexTop.y += halfDeltaHeight;
				vertexBottom.y -= halfDeltaHeight;
				vertices [ lastVertexIndex ] = vertexTop;
				vertices [ lastVertexIndex + 1 ] = vertexBottom;

				mesh.SetVertices ( vertices );
				mesh.UploadMeshData ( false );
				mesh.RecalculateBounds ();

				UpdateComponents ();
				lastWaterSize = waterSize;
				waterPositionOfRest = halfWidth;
			}
		}

		//OnWillRenderObject is called for each camera if the object is visible
		void OnWillRenderObject ()
		{
			#if UNITY_EDITOR
			waterMaterial = meshRenderer.sharedMaterial;
			#endif

			if ( !waterMaterial )
				return;

			#if UNITY_EDITOR
			renderRefraction = waterMaterial.IsKeywordEnabled ( "Water2D_Refraction" );
			renderReflection = waterMaterial.IsKeywordEnabled ( "Water2D_Reflection" );
			#endif

			if ( !( renderReflection || renderRefraction ) )
				return;

			Camera currentCamera = Camera.current;

			//we only work with orthographic cameras!
			if ( !currentCamera || !currentCamera.orthographic )
				return;

			if ( !waterCamera ) {
				GameObject waterCameraGameObject = new GameObject ( "Water Camera For" + GetInstanceID () );
				//we will take care of creating and destroying this camera
				waterCameraGameObject.hideFlags = HideFlags.HideAndDontSave;
				waterCamera = waterCameraGameObject.AddComponent <Camera> ();
				//we will render this camera manually
				waterCameraGameObject.SetActive ( false );
				waterCamera.enabled = false;
				waterCamera.clearFlags = CameraClearFlags.SolidColor;
				waterCamera.orthographic = true;
				waterCamera.nearClipPlane = 0.03f;
				waterCamera.farClipPlane = farClipPlane;
				waterCamera.allowHDR = allowHDR;
				waterCamera.allowMSAA = allowMSAA;
			}

			//we get water bounds in world space
			Bounds bounds = meshRenderer.bounds;

			Vector3 min, max;
			min = bounds.min;
			max = bounds.max;

			if ( Mathf.Approximately ( min.x, max.x ) || Mathf.Approximately ( min.y, max.y ) )
				return;

			min = currentCamera.WorldToScreenPoint ( min );
			max = currentCamera.WorldToScreenPoint ( max );

			//when the size or the position of the water changes, or the current camera moves , we update our render settings
			updateWaterRenderSettings = min != lastBoundsScreenSpaceMin || max != lastBoundsScreenSpaceMax;

			#if UNITY_EDITOR
			if ( !Application.isPlaying ) {
				updateWaterRenderSettings = true;
			}
			#endif

			if ( updateWaterRenderSettings ) {
				lastBoundsScreenSpaceMin = min;
				lastBoundsScreenSpaceMax = max;
				updateWaterRenderSettings = false;

				int cameraWidth = currentCamera.pixelWidth;
				int cameraHeight = currentCamera.pixelHeight;

				//We only render the visible part of the water to improve performance.
				if ( min.x < 0f ) {
					min.x = 0f;
				}

				if ( max.x > cameraWidth ) {
					max.x = cameraWidth;
				}

				if ( min.y < 0f ) {
					min.y = 0f;
				}

				if ( max.y > cameraHeight ) {
					max.y = cameraHeight;
				}

				int textureWidth = Mathf.RoundToInt ( max.x - min.x );
				int textureHeight = Mathf.RoundToInt ( max.y - min.y );

				min = currentCamera.ScreenToWorldPoint ( min );
				max = currentCamera.ScreenToWorldPoint ( max );

				Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
				Matrix4x4 waterMatrix = Matrix4x4.Ortho ( min.x, max.x, min.y, max.y, 0.03f, farClipPlane );
				waterCamera.projectionMatrix = waterMatrix;
				waterMaterial.SetMatrix ( waterMatrixID, waterMatrix * localToWorldMatrix );

				float zPosition = transform.position.z;
				if ( renderRefraction ) {
					waterCameraPositionForRefractionRendering.x = 0f;
					waterCameraPositionForRefractionRendering.y = 0f;
					waterCameraPositionForRefractionRendering.z = zPosition;

					int refractionTextureWidth = Mathf.RoundToInt ( textureWidth * refractionRenderTextureResizeFactor );
					int refractionTextureHeight = Mathf.RoundToInt ( textureHeight * refractionRenderTextureResizeFactor );

					if ( refractionTextureWidth < 1 || refractionTextureHeight < 1 )
						return;

					if ( refractionRenderTexture ) {
						RenderTexture.ReleaseTemporary ( refractionRenderTexture );
					}
					refractionRenderTexture = RenderTexture.GetTemporary ( refractionTextureWidth, refractionTextureHeight, 16 );

					waterMaterial.SetTexture ( refractionTextureID, refractionRenderTexture );
				}

				if ( renderReflection ) {
					waterCameraPositionForReflectionRendering.x = 0f;
					//we position the water camera just above the water surface
					waterCameraPositionForReflectionRendering.y = -( max.y + min.y ) + localToWorldMatrix.m11 * waterSize.y + 2f * localToWorldMatrix.m13;
					waterCameraPositionForReflectionRendering.z = zPosition + reflectionZOffset;

					int reflectionTextureWidth = Mathf.RoundToInt ( textureWidth * reflectionRenderTextureResizeFactor );
					int reflectionTextureHeight = Mathf.RoundToInt ( textureHeight * reflectionRenderTextureResizeFactor );

					if ( reflectionTextureWidth < 1 || reflectionTextureHeight < 1 )
						return;

					if ( reflectionRenderTexture ) {
						RenderTexture.ReleaseTemporary ( reflectionRenderTexture );
					}
					reflectionRenderTexture = RenderTexture.GetTemporary ( reflectionTextureWidth, reflectionTextureHeight, 16 );

					waterMaterial.SetTexture ( reflectionTextureID, reflectionRenderTexture );
				}
			}

			int pixelLightsCount = QualitySettings.pixelLightCount;
			if ( !renderPixelLights ) {
				QualitySettings.pixelLightCount = 0;
			}

			waterCamera.backgroundColor = currentCamera.backgroundColor;

			if ( renderRefraction ) {
				waterCamera.cullingMask = refractionCullingMask;
				waterCamera.targetTexture = refractionRenderTexture;
				waterCamera.transform.position = waterCameraPositionForRefractionRendering;
				waterCamera.Render ();
			}

			if ( renderReflection ) {
				waterCamera.cullingMask = reflectionCullingMask;
				waterCamera.transform.position = waterCameraPositionForReflectionRendering;
				waterCamera.targetTexture = reflectionRenderTexture;
				waterCamera.Render ();
			}

			QualitySettings.pixelLightCount = pixelLightsCount;
		}

		void PlaySplashSound ( float velocity )
		{
			if ( !audioSource )
				return;
			
			float interpolationValue = 1f - ( velocity - minimumDisturbance ) / maximumDisturbance;
			audioSource.pitch = Mathf.Lerp ( minimumAudioPitch, maximumAudioPitch, interpolationValue );
			audioSource.PlayOneShot ( splashAudioClip );
		}

		public void RecomputeMesh ()
		{
			if ( !mesh || !meshFilter.sharedMesh ) {
				mesh = new Mesh ();
				mesh.MarkDynamic ();
				mesh.hideFlags = HideFlags.HideAndDontSave;
				mesh.name = "Water2D Mesh";
				meshFilter.sharedMesh = mesh;
			}

			float halfWidth = waterSize.x / 2f;
			float halfHeight = waterSize.y / 2f;

			float activeWaterSurfaceWidth;
			float xStep;
			float leftmostBoundary;
			float uStep;
			float leftmostBoundaryU;

			if ( useCustomBoundaries ) {
				activeWaterSurfaceWidth = Mathf.Abs ( secondCustomBoundary - firstCustomBoundary );
				surfaceVerticesCount = Mathf.RoundToInt ( activeWaterSurfaceWidth * subdivisionsCountPerUnit ) + 4;
				xStep = activeWaterSurfaceWidth / ( surfaceVerticesCount - 3 );
				leftmostBoundary = Mathf.Min ( secondCustomBoundary, firstCustomBoundary );
				uStep = ( activeWaterSurfaceWidth / waterSize.x ) / ( surfaceVerticesCount - 3 );
				leftmostBoundaryU = ( leftmostBoundary + halfWidth ) / waterSize.x;
			} else {
				activeWaterSurfaceWidth = 2f * halfWidth;
				surfaceVerticesCount = Mathf.RoundToInt ( waterSize.x * subdivisionsCountPerUnit ) + 2;
				xStep = activeWaterSurfaceWidth / ( surfaceVerticesCount - 1 );
				leftmostBoundary = -halfWidth + xStep;
				uStep = 1f / ( surfaceVerticesCount - 1 );
				leftmostBoundaryU = uStep;
			}

			vertices = new List<Vector3> ( surfaceVerticesCount * 2 );
			uvs = new List<Vector2> ( surfaceVerticesCount * 2 );
			triangles = new List<int> ( ( surfaceVerticesCount - 1 ) * 6 );
			velocities = new float[surfaceVerticesCount];

			vertices.Add ( new Vector3 ( -halfWidth, halfHeight ) );
			vertices.Add ( new Vector3 ( -halfWidth, -halfHeight ) );

			uvs.Add ( new Vector2 ( 0f, 1f ) );
			uvs.Add ( new Vector2 ( 0f, 0f ) );

			triangles.Add ( 0 );
			triangles.Add ( 2 );
			triangles.Add ( 3 );
			triangles.Add ( 0 );
			triangles.Add ( 3 );
			triangles.Add ( 1 );

			float xPosition = 0f;
			float uPosition = 0f;
			for ( int i = 1, index = 2, max = surfaceVerticesCount - 1 ; i < max ; i++,index += 2 ) {
				float x = xPosition + leftmostBoundary;
				xPosition += xStep;
				vertices.Add ( new Vector3 ( x, halfHeight ) );
				vertices.Add ( new Vector3 ( x, -halfHeight ) );

				float u = uPosition + leftmostBoundaryU;
				uPosition += uStep;
				uvs.Add ( new Vector2 ( u, 1f ) );
				uvs.Add ( new Vector2 ( u, 0f ) );

				triangles.Add ( index );
				triangles.Add ( index + 2 );
				triangles.Add ( index + 3 );
				triangles.Add ( index );
				triangles.Add ( index + 3 );
				triangles.Add ( index + 1 );
			}

			vertices.Add ( new Vector3 ( halfWidth, halfHeight ) );
			vertices.Add ( new Vector3 ( halfWidth, -halfHeight ) );

			uvs.Add ( new Vector2 ( 1f, 1f ) );
			uvs.Add ( new Vector2 ( 1f, 0f ) );

			mesh.Clear ();
			mesh.SetVertices ( vertices );
			mesh.SetUVs ( 0, uvs );
			mesh.SetTriangles ( triangles, 0 );
			mesh.RecalculateNormals ();

			UpdateComponents ();
			waterPositionOfRest = halfHeight;
			lastWaterSize = waterSize;
			if ( useCustomBoundaries ) {
				lastFirstCustomBoundary = firstCustomBoundary;
				lastSecondCustomBoundary = secondCustomBoundary;
			}
		}

		void UpdateComponents ()
		{
			float halfWidth = waterSize.x / 2f;
			float halfHeight = waterSize.y / 2f;
			boxCollider.size = waterSize;
			if ( edgeCollider ) {
				edgeColliderPoints [ 0 ].x = edgeColliderPoints [ 1 ].x = -halfWidth;
				edgeColliderPoints [ 2 ].x = edgeColliderPoints [ 3 ].x = halfWidth;

				edgeColliderPoints [ 0 ].y = edgeColliderPoints [ 3 ].y = halfHeight;
				edgeColliderPoints [ 1 ].y = edgeColliderPoints [ 2 ].y = -halfHeight;

				edgeCollider.points = edgeColliderPoints;
			}
			buoyancyEffector.surfaceLevel = waterSize.y * ( 0.5f - buoyancyEffectorSurfaceLevel );
		}

		#endregion
	}
}
