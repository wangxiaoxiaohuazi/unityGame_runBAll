using TTSDK;
using TTSDK.UNBridgeLib.LitJson;
using UnityEngine;

public class TTFunction : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
   
    }


    private void Start()
    {
        TT.InitSDK((code, env) =>
        {
            Debug.Log("Unity message init sdk callback");
            Debug.Log("Unity message code: " + code);
            Debug.Log("Unity message HostEnum: " + env.m_HostEnum);
        });

    }
    // Start is called before the first frame update
    public void TestSidebar()
    {
        var data = new JsonData
        {
            ["scene"] = "sidebar",
            //["activityId"] = "11111111",
        };
        TT.NavigateToScene(data, () =>
        {
            Debug.Log("navigate to scene success");
        }, () =>
        {
            Debug.Log("navigate to scene complete");
        }, (errCode, errMsg) =>
        {
            Debug.Log($"navigate to scene error, errCode:{errCode}, errMsg:{errMsg}");
        });
    }

}
