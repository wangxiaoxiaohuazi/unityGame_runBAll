
using UnityEngine;

public class SpeedLines : MonoBehaviour
{
    // 定义粒子系统组件
    private ParticleSystem ParticleSystem;
    // 定义粒子系统的发射属性
    private ParticleSystem.EmissionModule emission;
    // 定义粒子系统的渲染器组件
    private ParticleSystemRenderer ParticleRenderer;
    // 定义粒子系统的主属性
    private ParticleSystem.MainModule ParticlesMain;
    // 定义粒子数组
    private ParticleSystem.Particle[] particles;

    // 定义相机
    public new Transform camera;
    // 定义最小速度
    public float minSpeed = 41;

    // 定义生成距离
    public float spawnDistance = 10;
    // 定义范围
    public float margin = 2;
    // 定义生成宽度
    private float spawnWidth;

    // 定义位置更新延迟
    public float PositionUpdateDelay = 0.1f;
    // 定义位置更新时间剩余
    private float PositionUpdateTimeRemaining;
    // 定义是否初始化
    private bool isInitialized;
    // 定义上一次的位置
    private Vector3 lastpos;


    // 定义是否在运行时更新
    public bool UpdateAtRuntime;
    // 定义线条大小
    public float LinesSize = 0.02f;
    // 定义线条颜色1
    public Color LinesColor1 = new Color(1, 1, 1, 0.7f);
    // 定义线条颜色2
    public Color LinesColor2 = new Color(0.7f, 0.7f, 0.7f, 0.7f);
    // 定义线条数量
    public int LinesCount = 500;
    // 定义线条拉伸
    public float LinesStretching = 0.035f;


    // 开始方法
    void Start()
    {
        // 获取粒子系统组件
        ParticleSystem = GetComponent<ParticleSystem>();
        // 获取粒子系统的主属性
        ParticlesMain = ParticleSystem.main;
        // 获取粒子系统的发射属性
        emission = ParticleSystem.emission;
        // 获取粒子系统的渲染器组件
        ParticleRenderer = GetComponent<ParticleSystemRenderer>();

        // 设置粒子属性
        SetParticleProperties();

        // 计算生成宽度
        spawnWidth = spawnDistance * 2;
        // 获取粒子系统的形状属性
        var shape = ParticleSystem.shape;
        // 设置形状的缩放
        shape.scale = Vector3.one * spawnWidth;

        // 如果没有指定相机，则获取主相机
        if (!camera)
        {
            camera = Camera.main.transform;
        }
    }

    // 设置粒子属性
    private void SetParticleProperties()
    {
        // 设置粒子初始大小
        ParticlesMain.startSize = LinesSize;
        // 设置粒子初始颜色
        ParticlesMain.startColor = new ParticleSystem.MinMaxGradient(LinesColor1, LinesColor2);
        // 设置粒子发射速率
        emission.rateOverTime = LinesCount;
        // 设置粒子最大数量
        ParticlesMain.maxParticles = LinesCount * 2;
        // 设置粒子渲染速度
        ParticleRenderer.velocityScale = LinesStretching;
    }

    void LateUpdate() //has to happen after camera position is updated
    {
        //将粒子的位置设置为相机的位置
        transform.position = camera.position;

        //如果粒子系统没有初始化
        if (!isInitialized)
        {
            //记录粒子的初始位置
            lastpos = transform.position;
            //设置粒子系统已初始化
            isInitialized = true;
            //设置粒子系统更新时间
            PositionUpdateTimeRemaining = PositionUpdateDelay;
        }

        //如果粒子系统在运行时更新
        if (UpdateAtRuntime)
        {
            //设置粒子系统的属性
            SetParticleProperties();
        }

        //减少粒子系统更新时间
        PositionUpdateTimeRemaining -= Time.deltaTime;
        //如果粒子系统更新时间小于等于0
        if (PositionUpdateTimeRemaining <= 0)
        {
            //获取玩家对象
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            //获取玩家的速度
            var speed = player.GetComponent<SphereController>().forwardSpeed;
            //如果速度小于最小速度
            if (speed < minSpeed)
            {
                //停止粒子系统
                ParticleSystem.Stop();
                // Debug.Log("Stop");
            }
            else
            {
                //播放粒子系统
                ParticleSystem.Play();
                // Debug.Log("Play");
            }
            //记录粒子的位置
            lastpos = transform.position;
            //设置粒子系统更新时间
            PositionUpdateTimeRemaining = PositionUpdateDelay;
        }

        //如果粒子系统为空或者粒子数量小于粒子系统最大粒子数量
        if (particles == null || particles.Length < ParticleSystem.main.maxParticles)
            //创建粒子数组
            particles = new ParticleSystem.Particle[ParticleSystem.main.maxParticles];
        //获取粒子系统中的粒子数量
        int numParticlesAlive = ParticleSystem.GetParticles(particles);

        //定义粒子的位置和偏移量
        Vector3 pos;
        Vector3 offset;
        //遍历粒子系统中的粒子
        for (int i = 0; i < numParticlesAlive; i++)
        {
            //获取粒子的位置
            pos = particles[i].position;
            //计算粒子的偏移量
            offset = pos - transform.position;
            //如果粒子的x轴偏移量大于设定的范围
            if (Mathf.Abs(offset.x) > spawnDistance + margin)
            {
                //将粒子的x轴位置减去偏移量
                pos.x -= Mathf.Sign(offset.x) * spawnWidth;
            }
            //如果粒子的y轴偏移量大于设定的范围
            if (Mathf.Abs(offset.y) > spawnDistance + margin)
            {
                //将粒子的y轴位置减去偏移量
                pos.y -= Mathf.Sign(offset.y) * spawnWidth;
            }
            //如果粒子的z轴偏移量大于设定的范围
            if (Mathf.Abs(offset.z) > spawnDistance + margin)
            {
                //将粒子的z轴位置减去偏移量
                pos.z -= Mathf.Sign(offset.z) * spawnWidth;
            }
            //更新粒子的位置
            particles[i].position = pos;
        }
        //设置粒子系统的粒子
        ParticleSystem.SetParticles(particles, numParticlesAlive);
    }
}
