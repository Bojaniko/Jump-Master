#ifndef ToomRamp
#define ToonRamp

void ToonRamp_float(float3 Highlight, float3 Shadow, float Shades, float Position, out float3 ColorOut)
{
	half step = 1 / Shades;

	int steps = floor(Position / step);

	ColorOut = lerp(Shadow, Highlight, step * steps);
}

#endif