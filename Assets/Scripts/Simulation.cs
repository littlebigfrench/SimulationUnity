using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>> where TValue : System.IComparable
{
    public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
    {
        if (x.Value == null)
        {
            if (y.Value == null)
            {
                return 0;
            }
            return -1;
        }
        if (y.Value == null)
        {
            return 1;
        }

        return y.Value.CompareTo(x.Value);
    }
}
public class FloatComparerInvert<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>> where TValue : System.IComparable
{
    public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
    {
        if (x.Value == null)
        {
            if (y.Value == null)
            {
                return 0;
            }
            return -1;
        }
        if (y.Value == null)
        {
            return 1;
        }

        return x.Value.CompareTo(y.Value);
    }
}

public class SimulationTimer
{
    public float nextActionTime { get; set; }
    public float period { get; set; }

    public delegate void function();
    public function func;

    /*SimulationTimer test = new SimulationTimer();
    test.func = ResetPlayers;
        test.nextActionTime = 0.0f;
        test.period = 5f;
        timers.Add(timers.Count + 1, test);
        SimulationTimer test2 = new SimulationTimer();
    test2.func = ResetPlayerss;
        test2.nextActionTime = 0.0f;
        test2.period = 10f;
        timers.Add(timers.Count + 1, test2);*/
}

public class Simulation : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    private float countertimer = 0;

    public GameObject spawn;
    public GameObject prefab;
    public GameObject playersObject;

    public GameObject ui_timerreset_;
    public GameObject ui_far_;
    public GameObject ui_ascdesc_;
    public GameObject ui_originalspeed_;

    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public Dictionary<int, SimulationTimer> timers = new Dictionary<int, SimulationTimer>();

    public float moveSpeedMax = 255;
    public float maxDistanceMax = 8;

    public float numberofplayers = 10;
    public float timeruntilreset = 50;

    private TextMeshProUGUI ui_timerreset;
    private TextMeshProUGUI ui_far;
    private TextMeshProUGUI ui_ascdesc;
    private TextMeshProUGUI ui_originalspeed;

    private bool restarted = false;

    void Start()
    {
        ui_timerreset = ui_timerreset_.GetComponent<TextMeshProUGUI>();
        ui_far = ui_far_.GetComponent<TextMeshProUGUI>();
        ui_ascdesc = ui_ascdesc_.GetComponent<TextMeshProUGUI>();
        ui_originalspeed = ui_originalspeed_.GetComponent<TextMeshProUGUI>();
        SpawnPlayer();

        SimulationTimer resetplayers = new SimulationTimer();
        resetplayers.func = ResetPlayers;
        resetplayers.period = timeruntilreset;
        resetplayers.nextActionTime = resetplayers.period;
        timers.Add(timers.Count + 1, resetplayers);

        SimulationTimer timerreset = new SimulationTimer();
        timerreset.func = TimerReset;
        timerreset.period = 1f;
        timerreset.nextActionTime = timerreset.period;
        timers.Add(timers.Count + 1, timerreset);

        SimulationTimer fardistance = new SimulationTimer();
        fardistance.func = FarDistance;
        fardistance.period = 1f;
        fardistance.nextActionTime = fardistance.period;
        timers.Add(timers.Count + 1, fardistance);

        SimulationTimer ascdesc = new SimulationTimer();
        ascdesc.func = AscDesc;
        ascdesc.period = 1f;
        ascdesc.nextActionTime = ascdesc.period;
        timers.Add(timers.Count + 1, ascdesc);

        SimulationTimer originalspeed = new SimulationTimer();
        originalspeed.func = OriginalSpeed;
        originalspeed.period = 1f;
        originalspeed.nextActionTime = ascdesc.period;
        timers.Add(timers.Count + 1, originalspeed);
    }
    void OriginalSpeed()
    {
        if (restarted == true)
        {
            ui_originalspeed.text = "Original :\nspeed : " + players[1].GetComponent<Player>().moveSpeed + "\nfar from spawn : " + Vector3.Distance(players[1].transform.position, spawn.transform.position);
        }
        else
        {
            ui_originalspeed.text = "No original yet :(";
        }
    }
    void FarDistance()
    {
        List<KeyValuePair<int, float>> distances = new List<KeyValuePair<int, float>>();
        string distance = "Far from spawn:\n";
        int limit = 0;
        foreach (KeyValuePair<int, GameObject> entry in players)
        {
            GameObject player = entry.Value;
            distances.Add(new KeyValuePair<int, float>(entry.Key, Vector3.Distance(player.transform.position, spawn.transform.position)));
        }
        distances.Sort(new FloatComparer<int, float>());
        foreach (KeyValuePair<int, float> entry in distances)
        {
            if (limit == 0)
            {
                distance += "<color=\"red\">" + players[entry.Key].name + "</color> (" + players[entry.Key].GetComponent<Player>().moveSpeed + ") :\n" + entry.Value;
                limit++;
            }
            else if (limit <= 5)
            {
                distance += "\n<color=\"red\">" + players[entry.Key].name + "</color> (" + players[entry.Key].GetComponent<Player>().moveSpeed + ") :\n" + entry.Value;
                limit++;
            }
        }
        ui_far.text = distance;
    }
    void AscDesc()
    {
        List<KeyValuePair<int, float>> speeds = new List<KeyValuePair<int, float>>();
        string asc = "Ascendant speed:\n";
        string desc = "Descendant speed:\n";
        int limit = 0;
        foreach (KeyValuePair<int, GameObject> entry in players)
        {
            GameObject player = entry.Value;
            speeds.Add(new KeyValuePair<int, float>(entry.Key, player.GetComponent<Player>().moveSpeed));
        }
        speeds.Sort(new FloatComparer<int, float>());
        foreach (KeyValuePair<int, float> entry in speeds)
        {
            if ( limit == 0 )
            {
                asc += "<color=\"red\">" + players[entry.Key].name + "</color> : " + entry.Value;
                limit++;
            }
            else if (limit <= 5)
            {
                asc += "\n<color=\"red\">" + players[entry.Key].name + "</color> : " + entry.Value;
                limit++;
            }
        }
        limit = 0;
        speeds.Sort(new FloatComparerInvert<int, float>());
        foreach (KeyValuePair<int, float> entry in speeds)
        {
            if (limit == 0)
            {
                desc += "<color=\"red\">" + players[entry.Key].name + "</color> : " + entry.Value;
                limit++;
            }
            else if(limit <= 5)
            {
                desc += "\n<color=\"red\">" + players[entry.Key].name + "</color> : " + entry.Value;
                limit++;
            }
        }
        ui_ascdesc.text = asc+"\n"+desc;

    }
    void TimerReset()
    {
        countertimer++;
        ui_timerreset.text = countertimer + "/" + timeruntilreset + " secondes";
        if (countertimer >= timeruntilreset)
        {
            countertimer = 0;
        }
    }
    void ResetPlayers()
    {
        Debug.Log("Reseting ...");
        GameObject donotremove = spawn;
        foreach (KeyValuePair<int, GameObject> entry in players)
        {
            GameObject player = entry.Value;
            float playerdistance = Vector3.Distance(player.transform.position, spawn.transform.position);

            if (donotremove == spawn)
            {
                donotremove = player;
            }
            else
            {
                float donotremovedistance = Vector3.Distance(donotremove.transform.position, spawn.transform.position);

                if (playerdistance >= donotremovedistance)
                {
                    donotremove = player;
                }
            }
        }

        foreach (KeyValuePair<int, GameObject> entry in players)
        {
            GameObject player = entry.Value;
            if (player != donotremove)
            {
                Destroy(player);
            }
        }
        donotremove.name = "Original";
        players.Clear();
        players.Add(1, donotremove);
        SpawnPlayer();
    }
    void FixedUpdate()
    {
        Timer();
        MovePlayers();
        CheckPlayers();
    }
    void Timer()
    {
        foreach (KeyValuePair<int, SimulationTimer> entry in timers)
        {
            SimulationTimer timer = entry.Value;
            if (Time.time > timer.nextActionTime)
            {
                timer.nextActionTime += timer.period;
                timer.func();
            }
        }
    }
    private float coloNumberConversion(float num)
    {
        return (num / 255.0f);
    }
    void SpawnPlayer()
    {
        if (players.Count >= 1)
        {
            restarted = true;
            GameObject playerToClone = players[1];
            playerToClone.transform.position = spawn.transform.position;
            for (int i = 2; i < numberofplayers + 1 - 1; i++)
            {
                GameObject instance = Instantiate(playerToClone, spawn.transform.position, Quaternion.identity) as GameObject;
                Player pl = instance.GetComponent<Player>();
                pl.moveSpeed = pl.moveSpeed + Random.Range(-1 * moveSpeedMax / 10, moveSpeedMax / 10 + 1) / 2;
                if (pl.moveSpeed > moveSpeedMax)
                {
                    pl.moveSpeed = moveSpeedMax;
                }
                else if (pl.moveSpeed < 0)
                {
                    pl.moveSpeed = 0;
                }
                instance.name = "Player" + i;
                instance.transform.SetParent(playersObject.transform, true);
                Color color = instance.GetComponent<Renderer>().material.color;
                instance.GetComponent<Renderer>().material.color = new Color(Random.Range(pl.moveSpeed / 3f, pl.moveSpeed) / moveSpeedMax, Random.Range(pl.moveSpeed / 3f, pl.moveSpeed) / moveSpeedMax, Random.Range(pl.moveSpeed / 3f, pl.moveSpeed) / moveSpeedMax, 1);
                players.Add(i, instance);
            }
        }
        else
        {
            for (int i = 1; i < numberofplayers + 1; i++)
            {
                GameObject instance = Instantiate(prefab, spawn.transform.position, Quaternion.identity) as GameObject;
                Player pl = instance.GetComponent<Player>();
                pl.moveSpeed = Random.Range(0, moveSpeedMax / 10 + 1);
                instance.name = "Player" + i;
                instance.transform.SetParent(playersObject.transform, true);
                instance.GetComponent<Renderer>().material.color = new Color(Random.Range(pl.moveSpeed / 3f, pl.moveSpeed) / moveSpeedMax, Random.Range(pl.moveSpeed / 3f, pl.moveSpeed) / moveSpeedMax, Random.Range(pl.moveSpeed / 3f, pl.moveSpeed) / moveSpeedMax, 1);
                players.Add(i, instance);
            }
        }

        foreach (KeyValuePair<int, GameObject> entry in players)
        {
            GameObject player = entry.Value;
            foreach (KeyValuePair<int, GameObject> entry_ in players)
            {
                GameObject player_ = entry_.Value;
                Physics.IgnoreCollision(player.GetComponent<Collider>(), player_.GetComponent<Collider>());
            }
        }
    }
    void MovePlayers()
    {
        foreach (KeyValuePair<int, GameObject> entry in players)
        {
            GameObject player = entry.Value;
            Player pl = player.GetComponent<Player>();
            if (pl.transform.position.y >= -10 && pl.transform.position.y <= 10)
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                Vector3 tempVect = new Vector3(Random.Range(-1, 2) * pl.moveSpeed * Time.deltaTime, rb.velocity.y, Random.Range(-1, 2) * pl.moveSpeed * Time.deltaTime);
                rb.velocity = Vector3.SmoothDamp(rb.velocity, tempVect, ref velocity, .05f);
            }
        }
    }
    void CheckPlayers()
    {
        foreach (KeyValuePair<int, GameObject> entry in players)
        {
            GameObject player = entry.Value;
            if (-100 >= player.transform.position.y || 100 <= player.transform.position.y)
            {
                Debug.Log("A player go outside ! (" + player.transform.position.y + ") (" + entry.Key + ")");
                player.transform.position = spawn.transform.position;
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }
    }
}
