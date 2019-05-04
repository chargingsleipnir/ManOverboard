public class Yacht : Boat
{
    const int BUOYANCY_MAX = 4000;

    public override void OnStart() {
        if (startRan)
            return;

        Reset(BUOYANCY_MAX);
        startRan = true;
    }
}
