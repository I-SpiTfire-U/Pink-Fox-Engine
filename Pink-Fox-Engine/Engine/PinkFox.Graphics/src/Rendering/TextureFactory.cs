using System.Numerics;
using System.Runtime.InteropServices;
using PinkFox.Core.Types;
using SDL;

namespace PinkFox.Graphics.Rendering;

public static class TextureFactory
{

    public static unsafe Texture2D CreateRectangle(Renderer renderer, int width, int height, SDL_Color color)
    {
        Texture2D texture = new(renderer, width, height);

        texture.SetAsRenderTarget(renderer);
        SDL3.SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
        SDL3.SDL_RenderClear(renderer);
        SDL3.SDL_SetRenderTarget(renderer, null);

        return texture;
    }

    public static unsafe Texture2D CreateRectangleOutline(Renderer renderer, int width, int height, int thickness, SDL_Color color)
    {
        Texture2D texture = new(renderer, width, height);

        texture.SetAsRenderTarget(renderer);
        SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
        SDL3.SDL_RenderClear(renderer);

        SDL3.SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

        SDL_FRect top = new() { x = 0, y = 0, w = width, h = thickness };
        SDL_FRect bottom = new() { x = 0, y = height - thickness, w = width, h = thickness };
        SDL_FRect left = new() { x = 0, y = 0, w = thickness, h = height };
        SDL_FRect right = new() { x = width - thickness, y = 0, w = thickness, h = height };

        SDL3.SDL_RenderFillRect(renderer, &top);
        SDL3.SDL_RenderFillRect(renderer, &bottom);
        SDL3.SDL_RenderFillRect(renderer, &left);
        SDL3.SDL_RenderFillRect(renderer, &right);

        SDL3.SDL_SetRenderTarget(renderer, null);

        return texture;
    }

    public static unsafe Texture2D CreateCircle(Renderer renderer, int width, int height, SDL_Color color)
    {
        Texture2D texture = new(renderer, width, height);
        texture.SetAsRenderTarget(renderer);

        SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
        SDL3.SDL_RenderClear(renderer);
        SDL3.SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

        float centerX = width / 2f;
        float centerY = height / 2f;
        float radiusX = width / 2f;
        float radiusY = height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = (x - centerX) / radiusX;
                float dy = (y - centerY) / radiusY;
                if (dx * dx + dy * dy <= 1f)
                {
                    SDL_FRect point = new() { x = x, y = y, w = 1, h = 1 };
                    SDL3.SDL_RenderFillRect(renderer, &point);
                }
            }
        }

        SDL3.SDL_SetRenderTarget(renderer, null);

        return texture;
    }

    public static unsafe Texture2D CreateCircleOutline(Renderer renderer, int width, int height, int thickness, SDL_Color color)
    {
        Texture2D texture = new(renderer, width, height);
        texture.SetAsRenderTarget(renderer);

        SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
        SDL3.SDL_RenderClear(renderer);
        SDL3.SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

        float centerX = width / 2f;
        float centerY = height / 2f;
        float radiusX = width / 2f;
        float radiusY = height / 2f;
        float innerRadiusX = radiusX - thickness;
        float innerRadiusY = radiusY - thickness;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = (x - centerX) / radiusX;
                float dy = (y - centerY) / radiusY;
                float dist = MathF.Sqrt(dx * dx + dy * dy);

                float dxInner = (x - centerX) / innerRadiusX;
                float dyInner = (y - centerY) / innerRadiusY;
                float distInner = MathF.Sqrt(dxInner * dxInner + dyInner * dyInner);

                if (dist <= 1f && distInner >= 1f)
                {
                    SDL_FRect point = new() { x = x, y = y, w = 1, h = 1 };
                    SDL3.SDL_RenderFillRect(renderer, &point);
                }
            }
        }

        SDL3.SDL_SetRenderTarget(renderer, null);

        return texture;
    }

    public static unsafe Texture2D CreateRightAngledTriangle(Renderer renderer, int width, int height, SDL_Color color)
    {
        Texture2D texture = new(renderer, width, height);

        texture.SetAsRenderTarget(renderer);
        SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
        SDL3.SDL_RenderClear(renderer);

        SDL3.SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

        for (int y = 0; y < height; y++)
        {
            int maxX = y * width / height; // right edge of the triangle at this row
            for (int x = 0; x <= maxX; x++)
            {
                SDL_FRect point = new() { x = x, y = y, w = 1, h = 1 };
                SDL3.SDL_RenderFillRect(renderer, &point);
            }
        }

        SDL3.SDL_SetRenderTarget(renderer, null);

        return texture;
    }
    
public static unsafe Texture2D CreateRightAngledTriangleOutline(Renderer renderer, int width, int height, int thickness, SDL_Color color)
{
    Texture2D texture = new(renderer, width, height);
    texture.SetAsRenderTarget(renderer);

    SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
    SDL3.SDL_RenderClear(renderer);
    SDL3.SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

    float diagLength = MathF.Sqrt(width * width + height * height);

    for (int y = 0; y < height; y++)
    {
        int maxX = y * width / height;

        for (int x = 0; x <= maxX; x++)
        {
            // Distance to hypotenuse
            float distHyp = MathF.Abs(height * x - width * y) / diagLength;

            // Distance to edges
            bool onLeftEdge = x < thickness;
            bool onBottomEdge = y >= height - thickness;
            bool onHypotenuse = distHyp <= thickness;

            if (onLeftEdge || onBottomEdge || onHypotenuse)
            {
                SDL_FRect point = new() { x = x, y = y, w = 1, h = 1 };
                SDL3.SDL_RenderFillRect(renderer, &point);
            }
        }
    }

    SDL3.SDL_SetRenderTarget(renderer, null);
    return texture;
}


    public static unsafe Texture2D CreateEquilateralTriangle(Renderer renderer, int width, int height, SDL_Color color)
    {
        Texture2D texture = new(renderer, width, height);

        texture.SetAsRenderTarget(renderer);
        SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
        SDL3.SDL_RenderClear(renderer);

        SDL3.SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

        float halfWidth = width / 2f;

        for (int y = 0; y < height; y++)
        {
            float ny = (float)y / (height - 1);
            int leftBound = (int)(halfWidth - ny * halfWidth);
            int rightBound = (int)(halfWidth + ny * halfWidth);

            for (int x = leftBound; x <= rightBound; x++)
            {
                SDL_FRect point = new() { x = x, y = y, w = 1, h = 1 };
                SDL3.SDL_RenderFillRect(renderer, &point);
            }
        }

        SDL3.SDL_SetRenderTarget(renderer, null);

        return texture;
    }


public static unsafe Texture2D CreateEquilateralTriangleOutline(Renderer renderer, int width, int height, int thickness, SDL_Color color)
{
    Texture2D texture = new(renderer, width, height);
    texture.SetAsRenderTarget(renderer);

    SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
    SDL3.SDL_RenderClear(renderer);
    SDL3.SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

    float halfWidth = width / 2f;

    for (int y = 0; y < height; y++)
    {
        float ny = (float)y / (height - 1);
        int leftBound = (int)(halfWidth - ny * halfWidth);
        int rightBound = (int)(halfWidth + ny * halfWidth);

        // Draw the left and right edges only
        for (int t = 0; t < thickness; t++)
        {
            if (leftBound + t <= rightBound - t)
            {
                SDL_FRect leftEdge = new() { x = leftBound + t, y = y, w = 1, h = 1 };
                SDL_FRect rightEdge = new() { x = rightBound - t, y = y, w = 1, h = 1 };
                SDL3.SDL_RenderFillRect(renderer, &leftEdge);
                SDL3.SDL_RenderFillRect(renderer, &rightEdge);
            }
        }
    }

    // Draw the bottom edge using the same left/right bounds as the last row
    float nyBottom = 1f; // bottom row
    int leftBoundBottom = (int)(halfWidth - nyBottom * halfWidth);
    int rightBoundBottom = (int)(halfWidth + nyBottom * halfWidth);

    for (int t = 0; t < thickness; t++)
    {
        SDL_FRect baseLine = new()
        {
            x = leftBoundBottom + t,
            y = height - 1 - t,
            w = rightBoundBottom - leftBoundBottom - 2 * t + 1,
            h = 1
        };
        SDL3.SDL_RenderFillRect(renderer, &baseLine);
    }

    SDL3.SDL_SetRenderTarget(renderer, null);
    return texture;
}

}