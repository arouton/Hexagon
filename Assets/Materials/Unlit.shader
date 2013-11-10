Shader "Custom/Unlit"
{
 	Properties
 	{
      _Color ("Color", Color) = (1.0, 1.0, 0.5, 1.0)
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input
      {
          float4 color : COLOR;
      };
      float4 _Color;
      void surf (Input IN, inout SurfaceOutput o)
      {
          o.Emission = _Color.rgb;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }