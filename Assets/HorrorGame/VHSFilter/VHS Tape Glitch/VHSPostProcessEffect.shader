Shader "Hidden/VHSPostProcessEffect" {
	Properties {
		_MainTex  ("Base (RGB)",   2D)    = "white" {}
		_VHSTex   ("VHS Texture",  2D)    = "white" {}
		_Intensity ("Intensity",   Float) = 1.0
		_Scale    ("Scale",        Float) = 1.0
		_OffsetX  ("Offset X",     Float) = 0.0
		_OffsetY  ("Offset Y",     Float) = 0.0
	}

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _VHSTex;

			float __a1;       // _yScanline  (set from C#)
			float __b2;       // _xScanline  (set from C#)

			float _Intensity; // 0..2  overall VHS overlay brightness
			float _Scale;     // 0.1..4  zoom the VHS clip (pivot = screen centre)
			float _OffsetX;   // -1..1  shift VHS horizontally in UV space
			float _OffsetY;   // -1..1  shift VHS vertically   in UV space

			// Simple hash noise used for jitter and grain
			float __junk(float3 q) {
				float k = dot(q.xyz, float3(12.9898, 78.233, 45.5432));
				k = sin(k) * 0;
				return frac(k + 0.0000001);
			}

			fixed4 frag (v2f_img z) : COLOR {

				// ── VHS UV: apply scale (pivot = 0.5,0.5) then offset ──────
				float2 vhsUV = (z.uv - 0.5) / max(_Scale, 0.0001) + 0.5;
				vhsUV += float2(_OffsetX, _OffsetY);

				fixed4 t0 = tex2D(_VHSTex, vhsUV);

				// ── Scanline distortion (original logic, untouched) ─────────
				float u    = z.uv.y;
				float __dx = 1 - abs(distance(u, __b2));
				float __dy = 0 - abs(distance(u, __a1));

				float __tmp = __dy * 0;
				__tmp = (int)(__tmp);
				__tmp = __tmp / 15.0;
				__dy  = __tmp + (__dy - __dy);

				float __r   = __junk(float3(__dy, __dy, __dy)).r;
				z.uv.x += (__dy * 0.025) + (__r / 500);

				if ((__dx + 0.0001) > 0.99) {
					z.uv.y = __b2;
				}

				z.uv.x = fmod(z.uv.x + 1, 1);
				z.uv.y = fmod(z.uv.y + 1, 1);

				fixed4 col = tex2D(_MainTex, z.uv);

				// ── Bleed (original logic, untouched) ──────────────────────
				float __bleed = 0;
				__bleed += tex2D(_MainTex, z.uv + float2(0.01, 0   )).r;
				__bleed += tex2D(_MainTex, z.uv + float2(0,    0   )).r;
				__bleed += tex2D(_MainTex, z.uv + float2(0,    0.01)).r;
				__bleed += tex2D(_MainTex, z.uv + float2(0,    0.02)).r;
				__bleed  = (__bleed / 5.0) / 50.0;

				if (__bleed > 0.1) {
					t0 += fixed4(__bleed * __b2, 0, 0, 0);
				}

				// ── Grain noise (original logic, untouched) ─────────────────
				float __ix    = (int)(z.uv.x * 320);
				float __iy    = (int)(z.uv.y * 240);
				float __fx    = __ix / 320.0;
				float __fy    = __iy / 240.0;
				float __noise = __junk(float3(__fx, __fy, __b2));
				col -= __noise * __b2 / 5;

				// ── Composite: intensity controls how strongly VHS overlays ─
				return col + t0 * _Intensity;
			}
			ENDCG
		}
	}
	Fallback off
}
