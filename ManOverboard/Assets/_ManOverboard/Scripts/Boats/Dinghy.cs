public class Dinghy : Boat {

    const int BUOYANCY_MAX = 1000;

    public override void OnStart() {
        if (startRan)
            return;

        Reset(BUOYANCY_MAX);
        startRan = true;
    }
}
