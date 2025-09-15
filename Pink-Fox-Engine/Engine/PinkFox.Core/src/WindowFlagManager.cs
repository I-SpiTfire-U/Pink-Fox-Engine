using PinkFox.Core.Types;
using SDL;

namespace PinkFox.Core;

public class WindowFlagManager
{
    public SDL_WindowFlags Flags { get; private set; }
    private readonly Window _Window;

    public unsafe WindowFlagManager(Window window, SDL_WindowFlags defaultFlags)
    {
        _Window = window;
        if (_Window.SdlWindow is not null)
        {
            Flags = SDL3.SDL_GetWindowFlags(_Window.SdlWindow);
            return;
        }
        Flags = defaultFlags == 0 ? 0 : defaultFlags;
    }

    public unsafe WindowFlagManager SetFullscreen(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_FULLSCREEN, value);
        SDL3.SDL_SetWindowFullscreen(_Window.SdlWindow, value);
        return this;
    }

    public unsafe WindowFlagManager SetResizable(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_RESIZABLE, value);
        SDL3.SDL_SetWindowResizable(_Window.SdlWindow, value);
        return this;
    }

    public unsafe WindowFlagManager SetBorderless(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_BORDERLESS, value);
        SDL3.SDL_SetWindowBordered(_Window.SdlWindow, !value);
        return this;
    }

    public unsafe WindowFlagManager SetHidden(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_HIDDEN, value);

        if (value)
        {
            SDL3.SDL_HideWindow(_Window.SdlWindow);
        }
        else
        {
            SDL3.SDL_ShowWindow(_Window.SdlWindow);
        }

        return this;
    }

    public unsafe WindowFlagManager SetMinimized(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_MINIMIZED, value);

        if (value)
        {
            SDL3.SDL_MinimizeWindow(_Window.SdlWindow);
        }
        else
        {
            SDL3.SDL_RestoreWindow(_Window.SdlWindow);
        }

        return this;
    }

    public unsafe WindowFlagManager SetMaximized(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_MAXIMIZED, value);

        if (value)
        {
            SDL3.SDL_MaximizeWindow(_Window.SdlWindow);
        }
        else
        {
            SDL3.SDL_RestoreWindow(_Window.SdlWindow);
        }

        return this;
    }

    public unsafe WindowFlagManager SetAlwaysOnTop(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP, value);
        SDL3.SDL_SetWindowAlwaysOnTop(_Window.SdlWindow, value);
        return this;
    }

    public unsafe WindowFlagManager SetUtility(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_UTILITY, value);
        return this;
    }

    public unsafe WindowFlagManager SetTooltip(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_TOOLTIP, value);
        return this;
    }

    public unsafe WindowFlagManager SetPopupMenu(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_POPUP_MENU, value);
        return this;
    }

    public unsafe WindowFlagManager SetMouseGrabbed(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_MOUSE_GRABBED, value);
        SDL3.SDL_SetWindowMouseGrab(_Window.SdlWindow, value);
        return this;
    }

    public unsafe WindowFlagManager SetKeyboardGrabbed(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_KEYBOARD_GRABBED, value);
        return this;
    }

    public unsafe WindowFlagManager SetMouseCapture(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_MOUSE_CAPTURE, value);
        SDL3.SDL_CaptureMouse(value);
        return this;
    }

    public unsafe WindowFlagManager SetMouseFocus(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS, value);
        return this;
    }

    public unsafe WindowFlagManager SetInputFocus(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS, value);
        return this;
    }

    public unsafe WindowFlagManager SetOccluded(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_OCCLUDED, value);
        return this;
    }

    public unsafe WindowFlagManager SetHighPixelDensity(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY, value);
        return this;
    }

    public unsafe WindowFlagManager SetVulkan(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_VULKAN, value);
        return this;
    }

    public unsafe WindowFlagManager SetMetal(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_METAL, value);
        return this;
    }

    public unsafe WindowFlagManager SetTransparent(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_TRANSPARENT, value);
        return this;
    }

    public unsafe WindowFlagManager SetNotFocusable(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_NOT_FOCUSABLE, value);
        return this;
    }

    public unsafe WindowFlagManager SetExternal(bool value)
    {
        SetFlag(SDL_WindowFlags.SDL_WINDOW_EXTERNAL, value);
        return this;
    }

    private void SetFlag(SDL_WindowFlags flag, bool value)
    {
        Flags = value ? Flags |= flag : Flags &= ~flag;
    }
}
