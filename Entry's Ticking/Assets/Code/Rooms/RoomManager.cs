using UnityEngine;
using Unity.Mathematics;

public class RoomManager : MonoBehaviour
{
    public RoomData[] roomPrefabs;
    RoomData current;
    int lastIndex = -1;

    public RoomData LoadRandomRoom()
    {
        if (current)
            Destroy(current.gameObject);
        
        int index;
        do {
            index = UnityEngine.Random.Range(0, roomPrefabs.Length);
        } while (roomPrefabs.Length > 1 && index == lastIndex);
        lastIndex = index;

        current = Instantiate(roomPrefabs[index], Vector3.zero, quaternion.identity);
        return current;
    }

    public RoomData ReloadCurrentRoom()
    {
        if (current)
            Destroy(current.gameObject);

        current = Instantiate(roomPrefabs[lastIndex], Vector3.zero, Quaternion.identity);
        return current;
    }
}
