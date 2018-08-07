#ifndef Game2D_WaterKit_INCLUDED
#define Game2D_WaterKit_INCLUDED

			#if defined(Water2D_Refraction) || defined(Water2D_Reflection)
				CBUFFER_START(Water2D_FrequentlyUpdatedVariables)
					#if defined(Water2D_Refraction)
					uniform sampler2D _RefractionTexture;
					#endif

					#if defined(Water2D_Reflection)
					uniform sampler2D _ReflectionTexture;
					#endif

					uniform float4x4 _WaterMVP;
				CBUFFER_END
			#endif

			#if defined(Water2D_Refraction) || defined(Water2D_Reflection) || (defined(Water2D_WaterTexture) && defined(Water2D_WaterNoise)) || (defined(Water2D_Surface) && defined(Water2D_SurfaceTexture) && defined(Water2D_SurfaceNoise))
				sampler2D _NoiseTexture:register(s0);
			#endif

			CBUFFER_START(Water2D_RarelyUpdatedVariables)
				#if defined(Water2D_Refraction) 
				half _RefractionNoiseSpeed;
				#endif

				#if defined(Water2D_Reflection) 
				half _ReflectionNoiseSpeed;

					#if defined(Water2D_Refraction)
						fixed _ReflectionVisibility;
					#endif

				#endif

				#if defined(Water2D_Surface)
				fixed _SurfaceLevel;
				fixed4 _SurfaceColor;

					#if defined(Water2D_SurfaceTexture)
						sampler2D _SurfaceTexture;
						float4 _SurfaceTexture_ST;
						fixed _SurfaceTextureOpacity;

						#if defined(Water2D_SurfaceNoise)
								half _SurfaceNoiseSpeed;
						#endif

					#endif

				#endif

				#if defined(Water2D_ColorGradient)
					fixed4 _WaterColorGradientStart;
					fixed4 _WaterColorGradientEnd;
				#else
					fixed4 _WaterColor;
				#endif

				#if defined(Water2D_WaterTexture)
					sampler2D _WaterTexture;
					float4 _WaterTexture_ST;
					fixed _WaterTextureOpacity;

						#if defined(Water2D_WaterNoise)
							half _WaterNoiseSpeed;
						#endif

				#endif

			CBUFFER_END

			struct water2D_VertexInput
			{
				float4 pos : POSITION;
				#if defined(Water2D_Surface) || defined(Water2D_ColorGradient)
				float2 uv : TEXCOORD0;
				#endif
				#if  defined(LIGHTMAP_ON)
				float2 lightmapCoord : TEXCOORD1;
				#endif
			};

			struct Water2D_VertexOutput
			{
				float4 pos : SV_POSITION;

				#if defined(Water2D_Surface) || defined(Water2D_ColorGradient)
				float2 uv : TEXCOORD0;
				#endif

				#if defined(Water2D_Refraction) || defined(Water2D_Reflection)
				float2 waterUv : TEXCOORD1 ;
				#endif

				#if defined(Water2D_Refraction)
				float2 refractionUv : TEXCOORD2;
				#endif

				#if defined(Water2D_Reflection) 
				float2 reflectionUv : TEXCOORD3;
				#endif

				#if defined(Water2D_WaterTexture)
					#if defined(Water2D_WaterNoise)
						float4 waterTextureUV : TEXCOORD4;
					#else
						float2 waterTextureUV : TEXCOORD4;
					#endif
				#endif

				#if defined(Water2D_Surface) && defined(Water2D_SurfaceTexture)
					#if defined(Water2D_SurfaceNoise)
						float4 surfaceTextureUV : TEXCOORD5;
					#else
						float2 surfaceTextureUV : TEXCOORD5;
					#endif
				#endif

				#if defined(LIGHTMAP_ON)
				    float2 lightmapCoord : TEXCOORD6;
				#else
					#if defined(UNITY_SHOULD_SAMPLE_SH)
			 		half3 sh : TEXCOORD6;
					#endif
				 #endif

				#if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_FORWARDADD)
  				float3 worldPos : TEXCOORD7;
 				#endif

				UNITY_FOG_COORDS(8)

				#if defined(Water2D_LIGHT_ON)
				fixed3 lightColor : COLOR0;
				#endif
			};

			inline Water2D_VertexOutput Water2D_Vert(water2D_VertexInput v){
				Water2D_VertexOutput o;
 				UNITY_INITIALIZE_OUTPUT(Water2D_VertexOutput,o);
				
				o.pos = UnityObjectToClipPos(v.pos.xyz);

				#if defined(Water2D_Surface) || defined(Water2D_ColorGradient)
					o.uv = v.uv;
				#endif

				#if defined(Water2D_Refraction) || defined(Water2D_Reflection)
					float4 pos = mul(_WaterMVP,v.pos);
					o.waterUv = pos.xy * 0.5 + 0.5;
				#endif

				#if defined(Water2D_Refraction) 
					o.refractionUv = v.pos.xy + _Time.w * _RefractionNoiseSpeed;
				#endif

				#if defined(Water2D_Reflection) 
					o.reflectionUv = v.pos.xy + _Time.w * _ReflectionNoiseSpeed;
				#endif

				#if defined(Water2D_Surface) && defined(Water2D_SurfaceTexture)
					o.surfaceTextureUV.xy = TRANSFORM_TEX(v.pos.xy,_SurfaceTexture);

					#if defined(Water2D_SurfaceNoise)
						o.surfaceTextureUV.zw = o.surfaceTextureUV.xy + _Time.w * _SurfaceNoiseSpeed;
					#endif

				#endif

				#if defined(Water2D_WaterTexture)
					o.waterTextureUV.xy = TRANSFORM_TEX(v.pos.xy,_WaterTexture);

					#if defined(Water2D_WaterNoise)
						o.waterTextureUV.zw = o.waterTextureUV.xy + _Time.w * _WaterNoiseSpeed;
					#endif

				#endif
				return o;
			}

			inline fixed4 Water2D_Frag(Water2D_VertexOutput i){
				fixed4 finalColor = fixed4(0.0,0.0,0.0,1.0);

				#if defined(Water2D_Surface)
					bool isSurface = false;
					if(i.uv.y > _SurfaceLevel){
						isSurface = true;
					}
				#endif


					//Refraction

					#if defined(Water2D_Refraction)
						float refractionDistortion = tex2D(_NoiseTexture,i.refractionUv).r;
						fixed4 refractionColor = tex2D(_RefractionTexture,float2(i.waterUv.xy + refractionDistortion));
						finalColor = refractionColor;
					#endif

					//Reflection

					#if defined(Water2D_Reflection)
						i.waterUv.y = 1.0 - i.waterUv.y;
						float reflectionDistortion = tex2D(_NoiseTexture,i.reflectionUv).g;
						fixed4 reflectionColor = tex2D(_ReflectionTexture,float2(i.waterUv.xy + reflectionDistortion));

						#if defined(Water2D_Refraction)
							finalColor.rgb += _ReflectionVisibility * reflectionColor.a * (reflectionColor.rgb - finalColor.rgb);
						#else
							finalColor = reflectionColor;
						#endif
					#endif

				//Applying surface texture

				#if defined (Water2D_Surface) && defined(Water2D_SurfaceTexture)
					if(isSurface){

						#if defined(Water2D_SurfaceNoise)
							i.surfaceTextureUV.xy += tex2D(_NoiseTexture,i.surfaceTextureUV.zw).b;
						#endif

						fixed4 sampledColor = tex2D(_SurfaceTexture,i.surfaceTextureUV.xy);

							#if defined(Water2D_Refraction) || defined(Water2D_Reflection)
								finalColor.rgb += _SurfaceTextureOpacity * sampledColor.a * (sampledColor.rgb - finalColor.rgb);
							#else 
								finalColor = _SurfaceTextureOpacity * sampledColor;
							#endif
					}
				#endif

				//Applying water texture

				#if defined(Water2D_WaterTexture)

					#if defined(Water2D_Surface)
						if(!isSurface){
					#endif

						#if defined(Water2D_WaterNoise)
							i.waterTextureUV.xy += tex2D(_NoiseTexture,i.waterTextureUV.zw).a;
						#endif

						fixed4 sampledColor = tex2D(_WaterTexture,i.waterTextureUV.xy);

							#if defined(Water2D_Refraction) || defined(Water2D_Reflection)
								finalColor.rgb += _WaterTextureOpacity * sampledColor.a * (sampledColor.rgb - finalColor.rgb);
							#else
								finalColor = _WaterTextureOpacity * sampledColor;
							#endif


					#if defined(Water2D_Surface)
							}
					#endif

				#endif


				//Appying surface color

				#if defined(Water2D_Surface)
					if(isSurface)
					{

						#if defined(Water2D_SurfaceTexture) || defined(Water2D_Refraction) || defined(Water2D_Reflection)
							finalColor += _SurfaceColor.a * (_SurfaceColor - finalColor);
								#else
									finalColor = _SurfaceColor;
						#endif

					}else{
				#endif

				//Applying water color

				fixed4 waterColor;
				#if defined(Water2D_ColorGradient)
					#if defined (Water2D_Surface)
						waterColor = _WaterColorGradientEnd + (i.uv.y / _SurfaceLevel) * (_WaterColorGradientStart - _WaterColorGradientEnd);
					#else
						waterColor = _WaterColorGradientEnd + i.uv.y * (_WaterColorGradientStart - _WaterColorGradientEnd);
					#endif
				#else
					waterColor = _WaterColor;
				#endif

				#if defined(Water2D_WaterTexture) || defined(Water2D_Refraction) || defined(Water2D_Reflection)
						finalColor += waterColor.a * (waterColor - finalColor);
							#else
								finalColor = waterColor;
							#endif


				#if defined(Water2D_Surface)
					}
				#endif

				return finalColor;
			}

	#if defined(Water2D_LIGHT_ON) && (defined(UNITY_PASS_VERTEX) || defined(UNITY_PASS_VERTEXLM))
			int4 unity_VertexLightParams;

			// ES2.0/WebGL/3DS can not do loops with non-constant-expression iteration counts.
			#if defined(SHADER_API_GLES)
			  #define LIGHT_LOOP_LIMIT 8
			#elif defined(SHADER_API_N3DS)
			  #define LIGHT_LOOP_LIMIT 4
			#else
			  #define LIGHT_LOOP_LIMIT unity_VertexLightParams.x // x: vertex lights count
			#endif

			// Compute attenuation & illumination from one light
			#if defined(POINT) || defined(SPOT)
				inline half3 computeLight(int idx, float3 mvPos) {
			#else
				inline half3 computeLight(int idx) {
			#endif
			  float4 lightPos = unity_LightPosition[idx];
			  float3 dirToLight = lightPos.xyz;
			  half attenuation = 1.0;

			  #if defined(POINT) || defined(SPOT)
			    dirToLight -= mvPos * lightPos.w;
			    // distance attenuation

			    half4 lightAtten = unity_LightAtten[idx];
			    float distSqr = dot(dirToLight, dirToLight);
			    if (lightPos.w != 0.0 && distSqr > lightAtten.w) 
			    	attenuation = 0.0; // set to 0 if outside of range
			    else
			    	attenuation /= (1.0 + lightAtten.z * distSqr);

			    distSqr = max(distSqr, 0.000001); // don't produce NaNs if some vertex position overlaps with the light 
			    dirToLight *= rsqrt(distSqr);
			    #if defined(SPOT)
			      // spot angle attenuation
			      half rho = max(dot(dirToLight, unity_SpotDirection[idx].xyz), 0.0);
			      half spotAtt = (rho - lightAtten.x) * lightAtten.y;
			      attenuation *= saturate(spotAtt);
			    #endif
			  #endif

			  // Compute illumination from one light, given attenuation
			  attenuation *= max(dirToLight.z, 0.0);
			  return attenuation * unity_LightColor[idx].rgb;
			}
	#endif //Water2D_LIGHT_ON && (UNITY_PASS_VERTEX || UNITY_PASS_VERTEXLM)

#endif // Game2D_WaterKit_INCLUDED
