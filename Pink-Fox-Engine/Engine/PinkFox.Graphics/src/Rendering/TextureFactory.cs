using System.Runtime.InteropServices;
using PinkFox.Core.Types;
using SDL;

namespace PinkFox.Graphics.Rendering;

public static class TextureFactory
{

    public static unsafe Texture2D CreateRectangle(Renderer renderer, int width, int height, SDL_Color color)
    {
        return CreateTextureFromGenerator(renderer, width, height, (ptr, offset, x, y) =>
        {
            SetPixel(ptr, offset, color.r, color.g, color.b, color.a);
        });
    }

    public static unsafe Texture2D CreateRectangleOutline(Renderer renderer, int width, int height, int outlineThickness, SDL_Color outlineColor)
    {
        return CreateTextureFromGenerator(renderer, width, height, (ptr, offset, x, y) =>
        {
            bool isOutline = x < outlineThickness || x >= width - outlineThickness ||
                             y < outlineThickness || y >= height - outlineThickness;

            if (isOutline)
            {
                SetPixel(ptr, offset, outlineColor.r, outlineColor.g, outlineColor.b, outlineColor.a);
            }
            else
            {
                SetPixel(ptr, offset, 0, 0, 0, 0);
            }
        });
    }

    public static unsafe Texture2D CreateRectangleGradient(Renderer renderer, int width, int height, SDL_Color color)
    {
        return CreateTextureFromGenerator(renderer, width, height, (ptr, offset, x, y) =>
        {
            float alphaFactor = 1f - ((float)y / (height - 1));
            byte alpha = (byte)(color.a * alphaFactor);

            SetPixel(ptr, offset, color.r, color.g, color.b, alpha);
        });
    }

    public static unsafe Texture2D CreateCircle(Renderer renderer, int width, int height, SDL_Color color)
    {
        float radiusX = width / 2f;
        float radiusY = height / 2f;
        float centerX = radiusX;
        float centerY = radiusY;

        return CreateTextureFromGenerator(renderer, width, height, (ptr, offset, x, y) =>
        {
            float dx = (x - centerX) / radiusX;
            float dy = (y - centerY) / radiusY;
            float distSq = dx * dx + dy * dy;

            if (distSq <= 1f)
            {
                SetPixel(ptr, offset, color.r, color.g, color.b, color.a);
            }
            else
            {
                SetPixel(ptr, offset, 0, 0, 0, 0);
            }
        });
    }

    public static unsafe Texture2D CreateCircleOutline(Renderer renderer, int width, int height, int outlineThickness, SDL_Color outlineColor)
    {
        float radiusX = width / 2f;
        float radiusY = height / 2f;
        float centerX = radiusX;
        float centerY = radiusY;

        float outlineThicknessX = outlineThickness / radiusX;
        float outlineThicknessY = outlineThickness / radiusY;

        return CreateTextureFromGenerator(renderer, width, height, (ptr, offset, x, y) =>
        {
            float dx = (x - centerX) / radiusX;
            float dy = (y - centerY) / radiusY;
            float dist = MathF.Sqrt(dx * dx + dy * dy);

            if (dist <= 1f && dist >= 1f - MathF.Max(outlineThicknessX, outlineThicknessY))
            {
                SetPixel(ptr, offset, outlineColor.r, outlineColor.g, outlineColor.b, outlineColor.a);
            }
            else
            {
                SetPixel(ptr, offset, 0, 0, 0, 0);
            }
        });
    }

    public static unsafe Texture2D CreateCircleGradient(Renderer renderer, int width, int height, SDL_Color color)
    {
        float radiusX = width / 2f;
        float radiusY = height / 2f;
        float centerX = width / 2f;
        float centerY = height / 2f;

        return CreateTextureFromGenerator(renderer, width, height, (ptr, offset, x, y) =>
        {
            float dx = (x - centerX) / radiusX;
            float dy = (y - centerY) / radiusY;
            float dist = MathF.Sqrt(dx * dx + dy * dy);

            if (dist <= 1f)
            {
                float alphaFactor = 1f - dist;
                byte alpha = (byte)(color.a * alphaFactor);

                SetPixel(ptr, offset, color.r, color.g, color.b, alpha);
            }
            else
            {
                SetPixel(ptr, offset, 0, 0, 0, 0);
            }
        });
    }

    private static unsafe Texture2D CreateTextureFromGenerator(Renderer renderer, int width, int height, PixelSetter pixelSetter)
    {
        const int bpp = 4;
        int pitch = width * bpp;
        byte* ptr = (byte*)Marshal.AllocHGlobal(height * pitch);

        try
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = (y * pitch) + (x * bpp);
                    pixelSetter(ptr, offset, x, y);
                }
            }

            return FromPixels(renderer, (nint)ptr, width, height, SDL_PixelFormat.SDL_PIXELFORMAT_RGBA8888);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)ptr);
        }
    }

    public static unsafe Texture2D CreateRightAngledTriangle(Renderer renderer, int width, int height, SDL_Color color)
    {
        return CreateTextureFromGenerator(renderer, width, height, (ptr, offset, x, y) =>
        {
            if (x <= y * width / height)
            {
                SetPixel(ptr, offset, color.r, color.g, color.b, color.a);
            }
            else
            {
                SetPixel(ptr, offset, 0, 0, 0, 0);
            }
        });
    }

    public static unsafe Texture2D CreateEquilateralTriangle(Renderer renderer, int width, int height, SDL_Color color)
    {
        float halfWidth = width / 2f;

        return CreateTextureFromGenerator(renderer, width, height, (ptr, offset, x, y) =>
        {
            float ny = (float)y / (height - 1);

            float leftBound = halfWidth - ny * halfWidth;
            float rightBound = halfWidth + ny * halfWidth;

            if (x >= leftBound && x <= rightBound)
            {
                SetPixel(ptr, offset, color.r, color.g, color.b, color.a);
            }
            else
            {
                SetPixel(ptr, offset, 0, 0, 0, 0);
            }
        });
    }

    private unsafe delegate void PixelSetter(byte* ptr, int offset, int x, int y);

    private static unsafe Texture2D FromPixels(Renderer renderer, nint pixels, int width, int height, SDL_PixelFormat format)
    {
        int pitch = width * SDL3.SDL_BYTESPERPIXEL(format);
        SDL_Surface* surface = SDL3.SDL_CreateSurfaceFrom(width, height, format, pixels, pitch);
        if (surface is null)
        {
            throw new Exception($"Failed to create surface from pixels: {SDL3.SDL_GetError()}");
        }
        return new Texture2D(surface, renderer);
    }

    private static unsafe void SetPixel(byte* ptr, int offset, byte r, byte g, byte b, byte a)
    {
        ptr[offset + 0] = a;
        ptr[offset + 1] = b;
        ptr[offset + 2] = g;
        ptr[offset + 3] = r;
    }
}