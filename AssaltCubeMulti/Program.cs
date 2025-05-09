﻿using AssaltCubeExternal;
using AssaltCubeMulti;
using ClickableTransparentOverlay;
using ImGuiNET;
using recoil_warzone_gui;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Vortice.Direct3D11;

class Program : Overlay
{
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    // Define uma estrutura para armazenar as coordenadas de um retângulo usado para obter o tamanho e a posição da janela do jogo.
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    // Obtém as coordenadas do retângulo da janela especificada pelo identificador hWnd e retorna uma estrutura RECT
    // com os valores Left, Top, Right e Bottom.
    public RECT GetWindowRect(IntPtr hWnd)
    {
        RECT rect; // Declara uma variável do tipo RECT para armazenar as coordenadas da janela.
        GetWindowRect(hWnd, out rect); // Chama a função GetWindowRect para preencher a estrutura rect com as coordenadas da janela.
        return rect; // Retorna a estrutura RECT preenchida.
    }

    MemoryHelper32 memory = new MemoryHelper32("ac_client");
    IntPtr moduleBase;

    public Entity LocalPlayer;
    public List<Entity> Entities = new List<Entity>();

    Vector2 windowLocation = new Vector2(0, 0);

    Vector2 windowSize = new Vector2(1920, 1080);
    Vector2 lineOrigin = new Vector2(1920 / 2, 1080);
    Vector2 windowCenter = new Vector2(1920 / 2, 1080 / 2);
    ImDrawListPtr inDrawListPtr = new ImDrawListPtr();

    public bool enableBox;
    public bool enableLine;
    public bool enableName;
    public bool enableHp;
    public bool enableAim;
    public bool enableInfiniteRifleAmmo;
    public bool enableInfiniteGranadeAmmo;

    Vector4 lineColor = new Vector4(1, 0, 0, 0.4f);
    Vector4 boxColor = new Vector4(1, 0, 0, 0.4f);
    Vector4 nameColor = new Vector4(1, 0, 0, 0.4f);
    Vector4 hpColor = new Vector4(1, 0, 0, 0.4f);


    static void Main(string[] args)
    {
        Program program = new Program();

        program.Start().Wait();

        Thread mainLogicThread = new Thread(program.MainLogic) { IsBackground = true };
        mainLogicThread.Start();
    }

    protected override void Render()
    {
        DrawMenu();
        DrawOverlay();
        Esp();
        ImGui.End();

    }

    uint selectedKey = KeysW32.Ctrl;
    int currentKeyIndex = 0;

    List<string> keyNames = new List<string>();
    List<uint> keyValues = new List<uint>();
    bool keysInitialized = false;

    void DrawMenu()
    {
        ImGui.SetNextWindowSize(new Vector2(300, 300));
        ImGui.Begin("AssaultCube");

        if (ImGui.BeginTabBar("Tabs"))
        {
            if (ImGui.BeginTabItem("Visuals"))
            {
                ImGui.Checkbox("Enable Box", ref enableBox);
                ImGui.Checkbox("Enable Line", ref enableLine);
                ImGui.Checkbox("Enable Name", ref enableName);
                ImGui.Checkbox("Enable HP", ref enableHp);

                ImGui.NewLine();


                ImGui.EndTabItem();
            }


            if (ImGui.BeginTabItem("Aim"))
            {
                ImGui.Checkbox("Enable Aim", ref enableAim);

                if (!keysInitialized)
                {
                    keyNames.Add("F1"); keyValues.Add(KeysW32.F1);
                    keyNames.Add("F2"); keyValues.Add(KeysW32.F2);
                    keyNames.Add("Insert"); keyValues.Add(KeysW32.Insert);
                    keyNames.Add("ArrowUp"); keyValues.Add(KeysW32.ArrowUp);
                    keyNames.Add("A"); keyValues.Add(KeysW32.A);
                    keyNames.Add("B"); keyValues.Add(KeysW32.B);
                    keyNames.Add("Enter"); keyValues.Add(KeysW32.Enter);
                    keyNames.Add("Escape"); keyValues.Add(KeysW32.Escape);
                    keyNames.Add("Space"); keyValues.Add(KeysW32.Space);
                    keyNames.Add("Left Mouse"); keyValues.Add(KeysW32.MouseLeft);
                    keyNames.Add("Right Mouse"); keyValues.Add(KeysW32.MouseRight);
                    keyNames.Add("Shift"); keyValues.Add(KeysW32.Shift);
                    keyNames.Add("Ctrl"); keyValues.Add(KeysW32.Ctrl);
                    keyNames.Add("Alt"); keyValues.Add(KeysW32.Alt);
                    keysInitialized = true;
                }
                currentKeyIndex = keyValues.IndexOf(selectedKey);
                if (ImGui.Combo("Aim Key", ref currentKeyIndex, keyNames.ToArray(), keyNames.Count))
                {
                    selectedKey = keyValues[currentKeyIndex];
                }

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Misc"))
            {
                ImGui.Checkbox("Infinite Rifle Ammo", ref enableInfiniteRifleAmmo);
                ImGui.Checkbox("Infinite Grenade Ammo", ref enableInfiniteGranadeAmmo);

                ImGui.EndTabItem();
            }


            if (ImGui.BeginTabItem("Colors"))
            {
                ImGui.ColorEdit4("Line Color", ref lineColor);
                ImGui.ColorEdit4("Box Color", ref boxColor);
                ImGui.ColorEdit4("Name Color", ref nameColor);
                ImGui.ColorEdit4("HP Color", ref hpColor);
                ImGui.EndTabItem();

            }

        }
        ImGui.EndTabBar();

    }

    void MainLogic()
    {
        moduleBase = memory.GetModuleBase(".exe");

        // Recupera as dimensões da janela do processo principal do jogo.
        var window = GetWindowRect(memory.GetProcess().MainWindowHandle);

        // Define a localização e o tamanho da janela do jogo com base nas dimensões obtidas.
        windowLocation = new Vector2(window.Left, window.Top);

        // Define o tamanho da janela com base nas dimensões obtidas.
        windowSize = Vector2.Subtract(new Vector2(window.Right, window.Bottom), windowLocation);

        // Define a origem da linha de visão do jogador local no centro da janela.
        lineOrigin = new Vector2(windowLocation.X + windowSize.X / 2, window.Bottom);

        windowCenter = new Vector2(windowLocation.X + windowSize.X / 2, windowLocation.Y + windowSize.Y / 2);

        while (true)
        {
            LocalPlayer = ReadLocalPlayer();
            Entities = ReadEntities(LocalPlayer);

            Entities = Entities.OrderBy(x => x.mag).ToList();

            if (enableInfiniteRifleAmmo)
                InfiniteRifleAmmo(LocalPlayer);

            if (enableInfiniteGranadeAmmo)
                InfiniteGranadeAmmo(LocalPlayer);

            if ((Win32.GetKeyState(selectedKey) & KeysW32.IsKeyPressed) > 0 && enableAim)
            {
                if (Entities.Count > 0)
                {
                    foreach (Entity entity in Entities)
                    {
                        var angles = CalcAngles(LocalPlayer, entity);
                        Aim(LocalPlayer, angles.X, angles.Y);
                        break;
                    }
                }
            }
        }

    }


    void Esp()
    {
        inDrawListPtr = ImGui.GetWindowDrawList();

        foreach (Entity entity in Entities)
        {
            var wtsFeet = Vector2.Add(WorldToScreen(ReadMatrix(), entity.feet, (int)windowSize.X, (int)windowSize.Y), windowLocation);
            var wtsHead = Vector2.Add(WorldToScreen(ReadMatrix(), entity.head, (int)windowSize.X, (int)windowSize.Y), windowLocation);

            var (boxStart, boxEnd) = CalculateBoundingBox(wtsHead, wtsFeet);

            if (wtsFeet.X > windowLocation.X)
            {
                if (enableLine)
                    DrawLine(lineOrigin, wtsFeet, ImGui.GetColorU32(lineColor));
                if (enableBox)
                    DrawBox(boxStart, boxEnd, ImGui.GetColorU32(boxColor));
                if (enableName)
                    DrawTextName(boxStart, boxEnd, entity.name, ImGui.GetColorU32(nameColor));
                if (enableHp)
                    DrawTextNameHp(boxStart, boxEnd, entity.health.ToString(), ImGui.GetColorU32(hpColor));
            }
        }

    }

    void InfiniteRifleAmmo(Entity localPlayer)
    {
        var rifleAmmo = memory.ReadInt(localPlayer.baseAddress, Offsets.vRifleAmmo);
        if (rifleAmmo < 100)
            memory.WriteInt(localPlayer.baseAddress, Offsets.vRifleAmmo, 100);
    }

    void InfiniteGranadeAmmo(Entity localPlayer)
    {
        var grenadeAmmo = memory.ReadInt(localPlayer.baseAddress, Offsets.vGrenadeAmmo);
        if (grenadeAmmo < 100)
            memory.WriteInt(localPlayer.baseAddress, Offsets.vGrenadeAmmo, 100);
    }

    void DrawTextName(Vector2 boxStart, Vector2 boxEnd, string text, uint color)
    {
        Vector2 textPosition = new Vector2(boxStart.X + (boxEnd.X - boxStart.X) / 2 - 10, boxStart.Y - 20); // Ajuste o valor 20 para mover o texto para cima ou para baixo conforme necessário.

        inDrawListPtr.AddText(textPosition, color, text);
    }

    void DrawTextNameHp(Vector2 boxStart, Vector2 boxEnd, string text, uint color)
    {
        Vector2 textPosition = new Vector2(boxStart.X + (boxEnd.X - boxStart.X) / 2 - 10, boxEnd.Y + 5);

        inDrawListPtr.AddText(textPosition, color, text);
    }

    void DrawBox(Vector2 boxStart, Vector2 boxEnd, uint color)
    {
        inDrawListPtr.AddRect(boxStart, boxEnd, color, 1.5f);
    }

    void DrawLine(Vector2 lineOrigin, Vector2 lineEnd, uint color)
    {
        inDrawListPtr.AddLine(lineOrigin, lineEnd, color, 2f);
    }



    public static (Vector2 boxStart, Vector2 boxEnd) CalculateBoundingBox(Vector2 headScreenPos, Vector2 feetScreenPos, float widthRatio = 0.5f)
    {
        float boxHeight = feetScreenPos.Y - 1 - headScreenPos.Y;
        float boxWidth = boxHeight * widthRatio;

        Vector2 boxStart = new Vector2(
            headScreenPos.X - boxWidth / 2,  // Centro X - metade da largura
            headScreenPos.Y                  // Topo da box na posição Y da cabeça
        );

        Vector2 boxEnd = new Vector2(
            headScreenPos.X + boxWidth / 2,  // Centro X + metade da largura
            feetScreenPos.Y                  // Base da box na posição Y dos pés
        );

        return (boxStart, boxEnd);
    }

    void DrawOverlay()
    {
        ImGui.SetNextWindowSize(windowSize);
        ImGui.SetNextWindowPos(windowLocation);
        ImGui.Begin("Overlay", ImGuiWindowFlags.NoDecoration
                         | ImGuiWindowFlags.NoBackground
                         | ImGuiWindowFlags.NoBringToFrontOnFocus
                         | ImGuiWindowFlags.NoMove
                         | ImGuiWindowFlags.NoInputs  // Evita que a janela receba interações
                         | ImGuiWindowFlags.NoCollapse
                         | ImGuiWindowFlags.NoScrollbar
                         | ImGuiWindowFlags.NoScrollWithMouse
                     );
    }

    public Vector2 WorldToScreen(ViewMatrix matrix, Vector3 pos, int width, int height)
    {
        Vector2 screenCoordinates = new Vector2();

        // Calcula as coordenadas da tela com base na matriz de visualização e na posição 3D.
        float screenW = (matrix.m14 * pos.X) + (matrix.m24 * pos.Y) + (matrix.m34 * pos.Z) + matrix.m44;

        // Verifica se a posição está na frente da câmera antes de calcular as coordenadas da tela.
        if (screenW > 0.001f)
        {
            // Calcula as coordenadas da tela com base na matriz de visualização e na posição 3D.
            float screenX = (matrix.m11 * pos.X) + (matrix.m21 * pos.Y) + (matrix.m31 * pos.Z) + matrix.m41;

            float screenY = (matrix.m12 * pos.X) + (matrix.m22 * pos.Y) + (matrix.m32 * pos.Z) + matrix.m42;

            float camX = width / 2f;
            float camY = height / 2f;

            float X = camX + (camX * screenX / screenW);
            float Y = camY - (camY * screenY / screenW);

            screenCoordinates.X = X;
            screenCoordinates.Y = Y;

            return screenCoordinates;
        }
        else
        {
            // Retorna uma posição inválida se a entidade estiver atrás da câmera.
            return new Vector2(-99, -99);
        }

    }

    public ViewMatrix ReadMatrix()
    {
        // Lê a matriz de visualização da memória do processo do jogo.
        var viewMatrix = new ViewMatrix();
        var floadMatrix = memory.ReadMatrix(moduleBase + Offsets.iViewMatrix);

        // Converte a matriz de float para a estrutura ViewMatrix.
        viewMatrix.m11 = floadMatrix[0];
        viewMatrix.m12 = floadMatrix[1];
        viewMatrix.m13 = floadMatrix[2];
        viewMatrix.m14 = floadMatrix[3];

        viewMatrix.m21 = floadMatrix[4];
        viewMatrix.m22 = floadMatrix[5];
        viewMatrix.m23 = floadMatrix[6];
        viewMatrix.m24 = floadMatrix[7];

        viewMatrix.m31 = floadMatrix[8];
        viewMatrix.m32 = floadMatrix[9];
        viewMatrix.m33 = floadMatrix[10];
        viewMatrix.m34 = floadMatrix[11];

        viewMatrix.m41 = floadMatrix[12];
        viewMatrix.m42 = floadMatrix[13];
        viewMatrix.m43 = floadMatrix[14];
        viewMatrix.m44 = floadMatrix[15];

        return viewMatrix;
    }

    public Vector2 CalcAngles(Entity localPlayer, Entity destEnt)
    {
        float x, y;

        var deltaX = destEnt.head.X - localPlayer.head.X;
        var deltaY = destEnt.head.Y - localPlayer.head.Y;

        x = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI) + 90;

        var deltaZ = destEnt.head.Z - localPlayer.head.Z;
        var dist = CalcDistance(localPlayer, destEnt);

        y = (float)(Math.Atan2(deltaZ, dist) * 180 / Math.PI);

        return new Vector2(x, y);
    }

    public void Aim(Entity ent, float x, float y)
    {
        memory.WriteFloat(ent.baseAddress, Offsets.vAngle, x);
        memory.WriteFloat(ent.baseAddress, Offsets.vAngle + 0x4, y);
    }

    public static float CalcDistance(Entity localPlayer, Entity destEnt)
    {
        return (float)
            Math.Sqrt(Math.Pow(destEnt.feet.X - localPlayer.feet.X, 2) +
                      Math.Pow(destEnt.feet.Y - localPlayer.feet.Y, 2)
                );
    }

    public List<Entity> ReadEntities(Entity localPlayer)
    {
        var entities = new List<Entity>();
        var entityListBase = memory.ReadPointer(moduleBase, Offsets.iEntityList);

        for (int i = 0; i < 12; i++)
        {
            var entityBase = memory.ReadPointer(entityListBase, (i * 0x4));
            if (entityBase == IntPtr.Zero) continue;
            Entity entity = ReadEntity(entityBase);
            if (entity.baseAddress == localPlayer.baseAddress) continue;
            if (entity.health <= 0 || entity.health > 100) continue;
            entity.mag = CalcMag(localPlayer, entity);
            entities.Add(entity);
        }
        return entities;
    }

    public static float CalcMag(Entity localPlayer, Entity destEnt)
    {
        return (float)
            Math.Sqrt(Math.Pow(destEnt.feet.X - localPlayer.feet.X, 2) +
                      Math.Pow(destEnt.feet.Y - localPlayer.feet.Y, 2) +
                      Math.Pow(destEnt.feet.Z - localPlayer.feet.Z, 2)
                );
    }

    public Entity ReadLocalPlayer()
    {
        var localPlayerBase = ReadEntity(memory.ReadPointer(moduleBase, Offsets.iLocalPlayer));
        localPlayerBase.viewAngles.X = memory.ReadFloat(localPlayerBase.baseAddress, Offsets.vAngle);
        localPlayerBase.viewAngles.Y = memory.ReadFloat(localPlayerBase.baseAddress, Offsets.vAngle + 0x4);

        return localPlayerBase;
    }

    public Entity ReadEntity(IntPtr entBase)
    {
        Entity entity = new Entity();
        entity.baseAddress = entBase;
        entity.health = memory.ReadInt(entBase, Offsets.vHealth);
        entity.head = memory.ReadVec(entBase, Offsets.vHead);
        entity.feet = memory.ReadVec(entBase, Offsets.vFeet);
        entity.name = Encoding.UTF8.GetString(memory.ReadBytes(entBase + Offsets.vName, 11));

        return entity;
    }



}