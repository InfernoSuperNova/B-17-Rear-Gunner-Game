Shader"Custom/CoreOuterTrail" 
{
    Properties
    {
        _ColorInside("Inside Color", Color) = (1, 0, 0, 1)
        _ColorOutside("Outside Color", Color) = (0, 0, 1, 1)
        _CutoffInside("Inside Cutoff", Range(-1, 1)) = 0.5
    }
 
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };
 
            float4 _ColorInside;
            float4 _ColorOutside;
            float _CutoffInside;
 
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }
 
            half4 frag(v2f i) : SV_Target
            {
                float distanceFromOrigin = length(i.texcoord);

                // Calculate the center of the teardrop shape
                float2 center = float2(0, 0);
                center.x = i.texcoord.x;
                //we should lerp between 0.5 and 0 based on x co ord because it gets thinner
                center.y = lerp(0.5, 1, i.texcoord.x);

                // Calculate the distance from the current fragment to the center
                float distance = length(i.texcoord - center);
                //we want to have a cutoff point before it starts transitioning to the outside color
                //basically if the distance is less than a certain amount, we want to make it 0
                //but without using an if statement

                distance = saturate(distance - _CutoffInside);

                // Interpolate between _ColorInside and _ColorOutside based on distance
                half4 finalColor = lerp(_ColorInside, _ColorOutside, distance);
                return finalColor;
            }
            ENDCG
        }
    }
}