public class Yacht : Boat
{
    const int BUOYANCY_MAX = 4000;

    public override void Start() {
        if (startRan)
            return;

        OnStart(BUOYANCY_MAX);
        startRan = true;
    }
}
