using System;

namespace AssaltCubeMulti
{
    public struct KeysW32
    {
        public const uint IsKeyPressed = 0x8000;
        public const uint IsKeyToggled = 0x0001;

        // Teclas de Função
        public const uint F1 = 0x70;
        public const uint F2 = 0x71;
        public const uint F3 = 0x72;
        public const uint F4 = 0x73;
        public const uint F5 = 0x74;
        public const uint F6 = 0x75;
        public const uint F7 = 0x76;
        public const uint F8 = 0x77;
        public const uint F9 = 0x78;
        public const uint F10 = 0x79;
        public const uint F11 = 0x7A;
        public const uint F12 = 0x7B;

        // Teclas de controle
        public const uint Insert = 0x2D;
        public const uint Home = 0x24;
        public const uint End = 0x23;
        public const uint PageUp = 0x21;
        public const uint PageDown = 0x22;
        public const uint Delete = 0x2E;
        public const uint Backspace = 0x08;

        // Teclas de navegação
        public const uint ArrowUp = 0x26;
        public const uint ArrowDown = 0x28;
        public const uint ArrowLeft = 0x25;
        public const uint ArrowRight = 0x27;

        // Teclas de números
        public const uint Num0 = 0x30;
        public const uint Num1 = 0x31;
        public const uint Num2 = 0x32;
        public const uint Num3 = 0x33;
        public const uint Num4 = 0x34;
        public const uint Num5 = 0x35;
        public const uint Num6 = 0x36;
        public const uint Num7 = 0x37;
        public const uint Num8 = 0x38;
        public const uint Num9 = 0x39;

        // Teclas alfabéticas
        public const uint A = 0x41;
        public const uint B = 0x42;
        public const uint C = 0x43;
        public const uint D = 0x44;
        public const uint E = 0x45;
        public const uint F = 0x46;
        public const uint G = 0x47;
        public const uint H = 0x48;
        public const uint I = 0x49;
        public const uint J = 0x4A;
        public const uint K = 0x4B;
        public const uint L = 0x4C;
        public const uint M = 0x4D;
        public const uint N = 0x4E;
        public const uint O = 0x4F;
        public const uint P = 0x50;
        public const uint Q = 0x51;
        public const uint R = 0x52;
        public const uint S = 0x53;
        public const uint T = 0x54;
        public const uint U = 0x55;
        public const uint V = 0x56;
        public const uint W = 0x57;
        public const uint X = 0x58;
        public const uint Y = 0x59;
        public const uint Z = 0x5A;

        // Teclas de pontuação
        public const uint Space = 0x20;
        public const uint Enter = 0x0D;
        public const uint Escape = 0x1B;
        public const uint Tab = 0x09;

        // Teclado numérico (numpad)
        public const uint Numpad0 = 0x60;
        public const uint Numpad1 = 0x61;
        public const uint Numpad2 = 0x62;
        public const uint Numpad3 = 0x63;
        public const uint Numpad4 = 0x64;
        public const uint Numpad5 = 0x65;
        public const uint Numpad6 = 0x66;
        public const uint Numpad7 = 0x67;
        public const uint Numpad8 = 0x68;
        public const uint Numpad9 = 0x69;

        // Botões de mouse
        public const uint MouseLeft = 0x01;
        public const uint MouseRight = 0x02;
        public const uint MouseMiddle = 0x04;
        public const uint MouseXButton1 = 0x05;
        public const uint MouseXButton2 = 0x06;

        // Modificadores
        public const uint Shift = 0x10;
        public const uint Ctrl = 0x11;
        public const uint Alt = 0x12;

        // Misc
        public const uint CapsLock = 0x14;
        public const uint NumLock = 0x90;
        public const uint ScrollLock = 0x91;
    }
}
