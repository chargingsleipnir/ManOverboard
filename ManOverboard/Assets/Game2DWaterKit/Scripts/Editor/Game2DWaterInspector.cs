using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace Game2DWaterKit
{
	[CanEditMultipleObjects,CustomEditor ( typeof( Game2DWater ) )]
	class Game2DWaterInspector:Editor
	{
		#region variables

		private SerializedProperty subdivisionsCountPerUnit;
		private SerializedProperty waterSize;

		private SerializedProperty damping;
		private SerializedProperty stiffness;
		private SerializedProperty spread;
		private SerializedProperty useCustomBoundaries;
		private SerializedProperty firstCustomBoundary;
		private SerializedProperty secondCustomBoundary;
		private SerializedProperty buoyancyEffectorSurfaceLevel;

		private SerializedProperty collisionMask;
		private SerializedProperty collisionMinimumDepth;
		private SerializedProperty collisionMaximumDepth;
		private SerializedProperty minimumDisturbance;
		private SerializedProperty maximumDisturbance;
		private SerializedProperty velocityMultiplier;

		private SerializedProperty refractionRenderTextureResizeFactor;
		private SerializedProperty refractionCullingMask;
		private SerializedProperty reflectionRenderTextureResizeFactor;
		private SerializedProperty reflectionCullingMask;
		private SerializedProperty reflectionZOffset;

		private SerializedProperty renderPixelLights;
		private SerializedProperty sortingLayerID;
		private SerializedProperty sortingOrder;
		private SerializedProperty allowMSAA;
		private SerializedProperty allowHDR;
		private SerializedProperty farClipPlane;

		private SerializedProperty splashAudioClip;
		private SerializedProperty minimumAudioPitch;
		private SerializedProperty maximumAudioPitch;

		private static readonly GUIContent fixScalingButtonLabel = new GUIContent ( "Fix Scaling" );
		private static readonly GUIContent waterPropertiesFoldoutLabel = new GUIContent ( "Water Properties" );
		private static readonly GUIContent refractionPropertiesFoldoutLabel = new GUIContent ( "Refraction Properties" );
		private static readonly GUIContent reflectionPropertiesFoldoutLabel = new GUIContent ( "Reflection Properties" );
		private static readonly GUIContent renderingSettingsFoldoutLabel = new GUIContent ( "Rendering Settings" );
		private static readonly GUIContent audioSettingsFoldoutLabel = new GUIContent ( "Audio Settings" );

		private static readonly GUIContent waterSizeLabel = new GUIContent ( "Water Size", "Sets the water size. X represents the width and Y represents the height." );
		private static readonly GUIContent subdivisionsCountPerUnitLabel = new GUIContent ( "Subdivisions Per Unit", "Sets the number of water’s surface vertices within one unit." );
		private static readonly GUIContent useCustomBoundariesLabel = new GUIContent ( "Use Custom Boundaries", "Enable/Disable using custom wave boundaries. When waves reach a boundary, they bounce back." );
		private static readonly GUIContent firstCustomBoundaryLabel = new GUIContent ( "First Boundary", "The location of the first boundary." );
		private static readonly GUIContent secondCustomBoundaryLabel = new GUIContent ( "Second Boundary", "The location of the second boundary." );

		private static readonly GUIContent dampingLabel = new GUIContent ( "Damping", "Controls how fast the waves decay. A low value will make waves oscillate for a long time, while a high value will make waves oscillate for a short time." );
		private static readonly GUIContent spreadLabel = new GUIContent ( "Spread", "Controls how fast the waves spread." );
		private static readonly GUIContent stiffnessLabel = new GUIContent ( "Stiffness", "Controls the frequency of wave vibration. A low value will make waves oscillate slowly, while a high value will make waves oscillate quickly." );

		private static readonly GUIContent minimumDisturbanceLabel = new GUIContent ( "Minimum", "The minimum displacement of water’s surface when a GameObject falls into water." );
		private static readonly GUIContent maximumDisturbanceLabel = new GUIContent ( "Maximum", "The maximum displacement of water’s surface when a GameObject falls into water." );
		private static readonly GUIContent velocityMultiplierLabel = new GUIContent ( "Velocity Multiplier", "When a rigidbody falls into water, the amount of water’s surface displacement is determined by multiplying the rigidbody velocity by this factor." );
		private static readonly GUIContent buoyancyEffectorSurfaceLevelLabel = new GUIContent ( "Surface Level", "Sets the surface location of the buoyancy fluid. When a GameObject is above this line, no buoyancy forces are applied. When a GameObject is intersecting or completely below this line, buoyancy forces are applied." );
		private static readonly GUIContent collisionMinimumDepthLabel = new GUIContent ( "Minimum Depth", "Only GameObjects with Z coordinate (depth) greater than or equal to this value will disturb the water’s surface when they fall into water." );
		private static readonly GUIContent collisionMaximumDepthLabel = new GUIContent ( "Maximum Depth", "Only GameObjects with Z coordinate (depth) less than or equal to this value will disturb the water’s surface when they fall into water." );
		private static readonly GUIContent collisionMaskLabel = new GUIContent ( "Collision Mask", "Only GameObjects on these layers will disturb the water’s surface (produce waves) when they fall into water." );

		private static readonly GUIContent cameraCullingMaskLabel = new GUIContent ( "Culling Mask", "Only GameObjects on these layers will be rendered." );
		private static readonly GUIContent refractionRenderTextureResizeFactorLabel = new GUIContent ( "Resize Factor", "Specifies how much the RenderTexture used to render refraction is resized. Decreasing this value lowers the RenderTexture resolution and thus improves performance at the expense of visual quality." );
		private static readonly GUIContent reflectionRenderTextureResizeFactorLabel = new GUIContent ( "Resize Factor", "Specifies how much the RenderTexture used to render reflection is resized. Decreasing this value lowers the RenderTexture resolution and thus improves performance at the expense of visual quality." );
		private static readonly GUIContent reflectionZOffsetLabel = new GUIContent ( "Z Offset", "Controls where to start rendering reflection relative to the water GameObject position." );

		private static readonly GUIContent farClipPlaneLabel = new GUIContent ( "Far Clip Plane", "Sets the furthest point relative to the water that will be drawn when rendering refraction and/or reflection." );
		private static readonly GUIContent renderPixelLightsLabel = new GUIContent ( "Render Pixel Lights", "Controls whether the rendered objects will be affected by pixel lights. Disabling this could increase performance at the expense of visual fidelity." );
		private static readonly GUIContent sortingLayerLabel = new GUIContent ( "Sorting Layer", "The name of the water mesh renderer sorting layer." );
		private static readonly GUIContent orderInLayerLabel = new GUIContent ( "Order In Layer", "The water mesh renderer order within a sorting layer." );
		private static readonly GUIContent allowMSAALabel = new GUIContent ( "Allow MSAA", "Allow multisample antialiasing rendering." );
		private static readonly GUIContent allowHDRLabel = new GUIContent ( "Allow HDR", "Allow high dynamic range rendering." );
		private static readonly GUIContent useEdgeCollider2DLabel = new GUIContent ( "Use Edge Collider 2D", "Adds/Removes an EdgeCollider2D component. The points of the edge collider are automatically updated whenever the water size changes." );

		private static readonly GUIContent splashAudioClipLabel = new GUIContent ( "Splash Clip", "The AudioClip asset to play when a GameObject falls into water." );
		private static readonly GUIContent minimumAudioPitchLabel = new GUIContent ( "Minimum Pitch", "Sets the minimum frequency of the splash clip." );
		private static readonly GUIContent maximumAudioPicthLabel = new GUIContent ( "Maximum Pitch", "Sets the maximum frequency of the splash clip." );

		private static readonly string meshPropertiesLabel = "Mesh Properties";
		private static readonly string wavePropertiesLabel = "Wave Properties";
		private static readonly string disturbancePropertiesLabel = "Disturbance Properties";
		private static readonly string collisionPropertiesLabel = "Collision Properties";
		private static readonly string miscLabel = "Misc";

		private static readonly string audioPitchMessage = "The AudioSource pitch (playback speed) is linearly interpolated between the minimum pitch and the maximum pitch. When a GameObject falls into water, the higher its velocity, the lower the pitch value is.";
		private static readonly string nonUniformScaleWarning = "Unexpected water simulation results may occur when using non-uniform scaling.";
		private static readonly string audioSourceMesage = "Audio properties are hidden. Try adding an AudiSource component";
		private static readonly string refractionMessage = "Refraction properties are hidden. \"Refraction\" can be activated in the material editor";
		private static readonly string reflectionMessage = "Reflection properties are hidden. \"Reflection\" can be activated in the material editor";

		private static readonly Color wireframeColor = new Color ( 0.89f, 0.259f, 0.204f, 0.375f );
		private static readonly Color buoyancyEffectorSurfaceLevelGuidelineColor = Color.cyan;

		private AnimBool waterPropertiesExpanded = new AnimBool ();
		private AnimBool refractionPropertiesExpanded = new AnimBool ();
		private AnimBool reflectionPropertiesExpanded = new AnimBool ();
		private AnimBool renderingSettingsExpanded = new AnimBool ();
		private AnimBool audioSettingsExpanded = new AnimBool ();

		private bool isMultiEditing;

		#endregion

		#region Methods

		private void OnEnable ()
		{
			waterSize = serializedObject.FindProperty ( "waterSize" );
			subdivisionsCountPerUnit = serializedObject.FindProperty ( "subdivisionsCountPerUnit" );

			damping = serializedObject.FindProperty ( "damping" );
			stiffness = serializedObject.FindProperty ( "stiffness" );
			spread = serializedObject.FindProperty ( "spread" );
			useCustomBoundaries = serializedObject.FindProperty ( "useCustomBoundaries" );
			firstCustomBoundary = serializedObject.FindProperty ( "firstCustomBoundary" );
			secondCustomBoundary = serializedObject.FindProperty ( "secondCustomBoundary" );
			buoyancyEffectorSurfaceLevel = serializedObject.FindProperty ( "buoyancyEffectorSurfaceLevel" );

			collisionMask = serializedObject.FindProperty ( "collisionMask" );
			collisionMinimumDepth = serializedObject.FindProperty ( "collisionMinimumDepth" );
			collisionMaximumDepth = serializedObject.FindProperty ( "collisionMaximumDepth" );
			minimumDisturbance = serializedObject.FindProperty ( "minimumDisturbance" );
			maximumDisturbance = serializedObject.FindProperty ( "maximumDisturbance" );
			velocityMultiplier = serializedObject.FindProperty ( "velocityMultiplier" );

			refractionRenderTextureResizeFactor = serializedObject.FindProperty ( "refractionRenderTextureResizeFactor" );
			refractionCullingMask = serializedObject.FindProperty ( "refractionCullingMask" );
			reflectionRenderTextureResizeFactor = serializedObject.FindProperty ( "reflectionRenderTextureResizeFactor" );
			reflectionCullingMask = serializedObject.FindProperty ( "reflectionCullingMask" );
			reflectionZOffset = serializedObject.FindProperty ( "reflectionZOffset" );

			renderPixelLights = serializedObject.FindProperty ( "renderPixelLights" );
			sortingLayerID = serializedObject.FindProperty ( "sortingLayerID" );
			sortingOrder = serializedObject.FindProperty ( "sortingOrder" );
			allowMSAA = serializedObject.FindProperty ( "allowMSAA" );
			allowHDR = serializedObject.FindProperty ( "allowHDR" );
			farClipPlane = serializedObject.FindProperty ( "farClipPlane" );

			splashAudioClip = serializedObject.FindProperty ( "splashAudioClip" );
			minimumAudioPitch = serializedObject.FindProperty ( "minimumAudioPitch" );
			maximumAudioPitch = serializedObject.FindProperty ( "maximumAudioPitch" );

			waterPropertiesExpanded.valueChanged.AddListener ( new UnityAction ( Repaint ) );
			refractionPropertiesExpanded.valueChanged.AddListener ( new UnityAction ( Repaint ) );
			reflectionPropertiesExpanded.valueChanged.AddListener ( new UnityAction ( Repaint ) );
			renderingSettingsExpanded.valueChanged.AddListener ( new UnityAction ( Repaint ) );
			audioSettingsExpanded.valueChanged.AddListener ( new UnityAction ( Repaint ) );

			waterPropertiesExpanded.target = EditorPrefs.GetBool ( "Water2D_WaterPropertiesExpanded", false );
			refractionPropertiesExpanded.target = EditorPrefs.GetBool ( "Water2D_RefractionPropertiesExpanded", false );
			reflectionPropertiesExpanded.target = EditorPrefs.GetBool ( "Water2D_ReflectionPropertiesExpanded", false );
			renderingSettingsExpanded.target = EditorPrefs.GetBool ( "Water2D_RenderingSettingsExpanded", false );
			audioSettingsExpanded.target = EditorPrefs.GetBool ( "Water2D_AudioSettingsExpanded", false ); 

			isMultiEditing = targets.Length > 1;
		}

		private void OnDisable ()
		{
			waterPropertiesExpanded.valueChanged.RemoveListener ( new UnityAction ( Repaint ) );
			refractionPropertiesExpanded.valueChanged.RemoveListener ( new UnityAction ( Repaint ) );
			reflectionPropertiesExpanded.valueChanged.RemoveListener ( new UnityAction ( Repaint ) );
			renderingSettingsExpanded.valueChanged.RemoveListener ( new UnityAction ( Repaint ) );
			audioSettingsExpanded.valueChanged.RemoveListener ( new UnityAction ( Repaint ) );

			EditorPrefs.SetBool ( "Water2D_WaterPropertiesExpanded", waterPropertiesExpanded.target );
			EditorPrefs.SetBool ( "Water2D_RefractionPropertiesExpanded", refractionPropertiesExpanded.target );
			EditorPrefs.SetBool ( "Water2D_ReflectionPropertiesExpanded", reflectionPropertiesExpanded.target );
			EditorPrefs.SetBool ( "Water2D_RenderingSettingsExpanded", renderingSettingsExpanded.target );
			EditorPrefs.SetBool ( "Water2D_AudioSettingsExpanded", audioSettingsExpanded.target ); 
		}

		public override void OnInspectorGUI ()
		{
			Game2DWater water2D = target as Game2DWater;
			bool hasRefraction = false;
			bool hasReflection = false;
			bool hasAudioSource = water2D.GetComponent <AudioSource> () != null;

			Material water2DMaterial = water2D.GetComponent <MeshRenderer> ().sharedMaterial;
			if ( water2DMaterial ) {
				hasRefraction = water2DMaterial.IsKeywordEnabled ( "Water2D_Refraction" );
				hasReflection = water2DMaterial.IsKeywordEnabled ( "Water2D_Reflection" );
			}
			if ( PrefabUtility.GetPrefabType ( target ) == PrefabType.Prefab ) {
				hasRefraction = true;
				hasReflection = true;
			}

			serializedObject.Update ();

			if ( !isMultiEditing ) {
				DrawFixScalingField ();
			}

			waterPropertiesExpanded.target = EditorGUILayout.Foldout ( waterPropertiesExpanded.target, waterPropertiesFoldoutLabel, true );
			using ( var group = new EditorGUILayout.FadeGroupScope ( waterPropertiesExpanded.faded ) ) {
				if ( group.visible ) {
					EditorGUI.indentLevel++;
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.LabelField ( meshPropertiesLabel, EditorStyles.boldLabel );
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField ( waterSize, waterSizeLabel );
					EditorGUILayout.PropertyField ( subdivisionsCountPerUnit, subdivisionsCountPerUnitLabel );

					EditorGUILayout.Space ();
					EditorGUILayout.LabelField ( wavePropertiesLabel, EditorStyles.boldLabel );
					EditorGUILayout.PropertyField ( stiffness, stiffnessLabel );
					EditorGUILayout.PropertyField ( spread, spreadLabel );
					EditorGUILayout.Slider ( damping, 0f, 1f, dampingLabel );
					EditorGUILayout.PropertyField ( useCustomBoundaries, useCustomBoundariesLabel );
					if ( useCustomBoundaries.boolValue ) {
						EditorGUILayout.PropertyField ( firstCustomBoundary, firstCustomBoundaryLabel );
						EditorGUILayout.PropertyField ( secondCustomBoundary, secondCustomBoundaryLabel );
					}

					EditorGUILayout.Space ();
					EditorGUILayout.LabelField ( disturbancePropertiesLabel, EditorStyles.boldLabel );
					EditorGUILayout.PropertyField ( minimumDisturbance, minimumDisturbanceLabel );
					EditorGUILayout.PropertyField ( maximumDisturbance, maximumDisturbanceLabel );
					EditorGUILayout.PropertyField ( velocityMultiplier, velocityMultiplierLabel );

					EditorGUILayout.Space ();
					EditorGUILayout.LabelField ( collisionPropertiesLabel, EditorStyles.boldLabel );
					EditorGUILayout.PropertyField ( collisionMinimumDepth, collisionMinimumDepthLabel );
					EditorGUILayout.PropertyField ( collisionMaximumDepth, collisionMaximumDepthLabel );
					EditorGUILayout.PropertyField ( collisionMask, collisionMaskLabel );

					EditorGUILayout.Space ();
					EditorGUILayout.LabelField ( miscLabel, EditorStyles.boldLabel );
					EditorGUILayout.Slider ( buoyancyEffectorSurfaceLevel, 0f, 1f, buoyancyEffectorSurfaceLevelLabel );
					if ( !isMultiEditing )
						DrawEdgeColliderPropertyField ();

					EditorGUI.indentLevel--;
				}
			}

			if ( hasRefraction ) {
				refractionPropertiesExpanded.target = EditorGUILayout.Foldout ( refractionPropertiesExpanded.target, refractionPropertiesFoldoutLabel, true );
				using ( var group = new EditorGUILayout.FadeGroupScope ( refractionPropertiesExpanded.faded ) ) {
					if ( group.visible ) {
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField ( refractionCullingMask, cameraCullingMaskLabel );
						EditorGUILayout.Slider ( refractionRenderTextureResizeFactor, 0f, 1f, refractionRenderTextureResizeFactorLabel );
						EditorGUI.indentLevel--;
					}
				}
			}

			if ( hasReflection ) {
				reflectionPropertiesExpanded.target = EditorGUILayout.Foldout ( reflectionPropertiesExpanded.target, reflectionPropertiesFoldoutLabel, true );
				using ( var group = new EditorGUILayout.FadeGroupScope ( reflectionPropertiesExpanded.faded ) ) {
					if ( group.visible ) {
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField ( reflectionCullingMask, cameraCullingMaskLabel );
						EditorGUILayout.Slider ( reflectionRenderTextureResizeFactor, 0f, 1f, reflectionRenderTextureResizeFactorLabel );
						EditorGUILayout.PropertyField ( reflectionZOffset, reflectionZOffsetLabel );
						EditorGUI.indentLevel--;
					}
				}
			}

			renderingSettingsExpanded.target = EditorGUILayout.Foldout ( renderingSettingsExpanded.target, renderingSettingsFoldoutLabel, true );
			using ( var group = new EditorGUILayout.FadeGroupScope ( renderingSettingsExpanded.faded ) ) {
				if ( group.visible ) {
					EditorGUI.indentLevel++;
					if ( hasReflection || hasRefraction ) {
						EditorGUILayout.PropertyField ( farClipPlane, farClipPlaneLabel );
						EditorGUILayout.PropertyField ( renderPixelLights, renderPixelLightsLabel );
						EditorGUILayout.PropertyField ( allowMSAA, allowMSAALabel );
						EditorGUILayout.PropertyField ( allowHDR, allowHDRLabel );
					}
					DrawSortingLayerField ( sortingLayerID, sortingOrder );
					EditorGUI.indentLevel--;
				}
			}

			if ( hasAudioSource ) {
				audioSettingsExpanded.target = EditorGUILayout.Foldout ( audioSettingsExpanded.target, audioSettingsFoldoutLabel, true );
				using ( var group = new EditorGUILayout.FadeGroupScope ( audioSettingsExpanded.faded ) ) {
					if ( group.visible ) {
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField ( splashAudioClip, splashAudioClipLabel );
						EditorGUILayout.Slider ( minimumAudioPitch, -3f, 3f, minimumAudioPitchLabel );
						EditorGUILayout.Slider ( maximumAudioPitch, -3f, 3f, maximumAudioPicthLabel );
						EditorGUILayout.HelpBox ( audioPitchMessage, MessageType.None, true );
						EditorGUI.indentLevel--;
					}
				}
			} else {
				EditorGUILayout.HelpBox ( audioSourceMesage, MessageType.None, true );
			}

			if ( !hasRefraction )
				EditorGUILayout.HelpBox ( refractionMessage, MessageType.None, true );
			if ( !hasReflection )
				EditorGUILayout.HelpBox ( reflectionMessage, MessageType.None, true );

			serializedObject.ApplyModifiedProperties ();
		}

		private void OnSceneGUI ()
		{
			if ( !isMultiEditing ) {
				DrawWaterResizer ();
			}
			DrawWaterWireframe ();
			DrawBuoyancyEffectorSurfaceLevelGuideline ();
		}

		private void DrawWaterWireframe ()
		{
			Game2DWater water2D = target as Game2DWater;
			List<Vector3> vertices = new List<Vector3> ();
			water2D.GetComponent <MeshFilter> ().sharedMesh.GetVertices ( vertices );
			int start, end;
			if ( water2D.UseCustomBoundaries ) {
				start = 2;
				end = vertices.Count - 4;
			} else {
				start = 0;
				end = vertices.Count - 2;
			}
			Matrix4x4 localToWorldMatrix = water2D.transform.localToWorldMatrix;
			using ( new Handles.DrawingScope ( wireframeColor, localToWorldMatrix ) ) {
				for ( int i = start ; i <= end ; i += 2 ) {
					Handles.DrawLine ( vertices [ i ], vertices [ i + 1 ] );
				}
			}
		}

		private void DrawBuoyancyEffectorSurfaceLevelGuideline ()
		{
			Game2DWater water2D = target as Game2DWater;
			Vector2 size = water2D.WaterSize / 2f;
			float y = size.y * ( 1f - 2f * water2D.BuoyancyEffectorSurfaceLevel );
			Vector3 lineStart = water2D.transform.TransformPoint ( -size.x, y, 0f );
			Vector3 lineEnd = water2D.transform.TransformPoint ( size.x, y, 0f );
			Handles.color = buoyancyEffectorSurfaceLevelGuidelineColor;
			Handles.DrawLine ( lineStart, lineEnd );
			Handles.color = Color.white;
		}

		private void DrawWaterResizer ()
		{
			Game2DWater water2D = target as Game2DWater;
			Bounds bounds = water2D.GetComponent <MeshRenderer> ().bounds;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			Vector3 center = bounds.center;

			Vector3 upHandle = new Vector3 ( center.x, max.y, center.z );
			Vector3 downHandle = new Vector3 ( center.x, min.y, center.z );
			Vector3 rightHandle = new Vector3 ( max.x, center.y, center.z );
			Vector3 leftHandle = new Vector3 ( min.x, center.y, center.z );

			float handlesSize = HandleUtility.GetHandleSize ( center ) * 0.5f;
			EditorGUI.BeginChangeCheck ();
			Vector3 upPos = Handles.Slider ( upHandle, Vector3.up, handlesSize, Handles.ArrowHandleCap, 1f );
			Vector3 downPos = Handles.Slider ( downHandle, Vector3.down, handlesSize, Handles.ArrowHandleCap, 1f );
			Vector3 rightPos = Handles.Slider ( rightHandle, Vector3.right, handlesSize, Handles.ArrowHandleCap, 1f );
			Vector3 leftPos = Handles.Slider ( leftHandle, Vector3.left, handlesSize, Handles.ArrowHandleCap, 1f );
			if ( EditorGUI.EndChangeCheck () ) {
				Undo.RecordObject ( water2D, "changing water size" );
				Vector3 newCenter = new Vector3 ( ( rightPos.x + leftPos.x ) / 2f, ( upPos.y + downPos.y ) / 2f, center.z );
				upPos = water2D.transform.worldToLocalMatrix.MultiplyPoint ( upPos );
				downPos = water2D.transform.worldToLocalMatrix.MultiplyPoint ( downPos );
				rightPos = water2D.transform.worldToLocalMatrix.MultiplyPoint ( rightPos );
				leftPos = water2D.transform.worldToLocalMatrix.MultiplyPoint ( leftPos );
				Vector2 newSize = new Vector2 ( Mathf.Clamp ( rightPos.x - leftPos.x, 0f, float.MaxValue ), Mathf.Clamp ( upPos.y - downPos.y, 0f, float.MaxValue ) );
				if ( newSize.x > 0f && newSize.y > 0f ) {
					if ( water2D.UseCustomBoundaries ) {
						float halfWidth = newSize.x / 2f;
						water2D.SecondCustomBoundary = Mathf.Clamp ( water2D.SecondCustomBoundary, -halfWidth, halfWidth );
						water2D.FirstCustomBoundary = Mathf.Clamp ( water2D.FirstCustomBoundary, -halfWidth, halfWidth );
					}
					water2D.WaterSize = newSize;
					water2D.transform.position = newCenter;
					EditorUtility.SetDirty ( water2D );
				}
			}

			if ( GUI.changed )
				SceneView.RepaintAll ();
		}

		void DrawEdgeColliderPropertyField ()
		{
			Game2DWater water2D = target as Game2DWater;
			EditorGUI.BeginChangeCheck ();
			bool hasEdgeCollider = EditorGUILayout.Toggle ( useEdgeCollider2DLabel, water2D.GetComponent <EdgeCollider2D> () != null );
			if ( EditorGUI.EndChangeCheck () ) {
				if ( hasEdgeCollider ) {
					EdgeCollider2D edgeCollider = water2D.gameObject.AddComponent <EdgeCollider2D> ();
					float xOffset, yOffset;
					xOffset = -water2D.WaterSize.x / 2f;
					yOffset = water2D.WaterSize.y / 2f;
					edgeCollider.points = new [] {
						new Vector2 ( xOffset, yOffset ),
						new Vector2 ( xOffset, -yOffset ),
						new Vector2 ( -xOffset, -yOffset ),
						new Vector2 ( -xOffset, yOffset )
					};
					water2D.OnValidate ();
				} else {
					DestroyImmediate ( water2D.GetComponent <EdgeCollider2D> () );
				}
			}
		}

		static void DrawSortingLayerField ( SerializedProperty layerID , SerializedProperty orderInLayer )
		{
			MethodInfo methodInfo = typeof( EditorGUILayout ).GetMethod ( "SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, new [] {
				typeof( GUIContent ),
				typeof( SerializedProperty ),
				typeof( GUIStyle ),
				typeof( GUIStyle )
			}, null );

			if ( methodInfo != null ) {
				object[] parameters = { sortingLayerLabel, layerID, EditorStyles.popup, EditorStyles.label };
				methodInfo.Invoke ( null, parameters );
				EditorGUILayout.PropertyField ( orderInLayer, orderInLayerLabel );
			}

		}

		private bool DrawFixScalingField ()
		{
			Game2DWater water2D = target as Game2DWater;
			Vector2 scale = water2D.transform.localScale;
			if ( !Mathf.Approximately ( scale.x, 1f ) || !Mathf.Approximately ( scale.y, 1f ) ) {
				EditorGUILayout.HelpBox ( nonUniformScaleWarning, MessageType.Warning, true );
				if ( GUILayout.Button ( fixScalingButtonLabel ) ) {
					waterSize.vector2Value = Vector2.Scale ( waterSize.vector2Value, scale );
					water2D.transform.localScale = Vector3.one;
					return true;
				}
			}
			return false;
		}

		#endregion
	}
}