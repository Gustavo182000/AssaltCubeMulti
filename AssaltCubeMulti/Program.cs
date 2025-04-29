using AssaltCubeExternal;
using AssaltCubeMulti;
using ClickableTransparentOverlay;
using recoil_warzone_gui;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

class Program : Overlay
{
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);


    MemoryHelper32 memory = new MemoryHelper32("ac_client");
    IntPtr moduleBase;

    public Entity LocalPlayer;
    public List<Entity> Entities = new List<Entity>();

    static void Main(string[] args)
    {
        Program program = new Program();

        program.Start().Wait();

        Thread mainLogicThread = new Thread(program.MainLogic) { IsBackground = true };
        mainLogicThread.Start();
    }

    protected override void Render()
    {

    }

    void MainLogic()
    {
        moduleBase = memory.GetModuleBase(".exe");

        while (true)
        {
            LocalPlayer = ReadLocalPlayer();
            Entities = ReadEntities(LocalPlayer);

            Entities = Entities.OrderBy(x => x.mag).ToList();

            if ((Win32.GetKeyState(KeysW32.Ctrl) & KeysW32.IsKeyPressed) > 0)
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