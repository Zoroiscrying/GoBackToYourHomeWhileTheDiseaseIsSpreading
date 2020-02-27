using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MonsterLove.StateMachine;
using UnityEngine;

public class PedestrianGenerator : Singleton<PedestrianGenerator>
{
    public int PedestrianNumberMax = 10;
    private int m_currentGeneratedNum = 0;
    private int m_rollingNumber = 0;
    private int m_initPedNumMax;
    public float GenerateIntervalMax = 5f;

    public List<GameObject> NormalPedestrians = new List<GameObject>();
    public List<GameObject> MovablePedestriansWithoutProtection = new List<GameObject>();
    public List<GameObject> UnMovablePedestriansWithoutProtection = new List<GameObject>();
    public List<GameObject> Patients = new List<GameObject>();
    
    
    
    public enum PedestrianGenerationStates
    {
        Idle,
        Normal
    }
    
    private StateMachine<PedestrianGenerationStates> fsm;

    private void Awake()
    {
        fsm = StateMachine<PedestrianGenerationStates>.Initialize(this);
    }
    
    // Start is called before the first frame update
    public void Start()
    {
        m_initPedNumMax = this.PedestrianNumberMax;
        fsm.ChangeState(PedestrianGenerationStates.Idle);
    }

    public void GameStart()
    {
        fsm.ChangeState(PedestrianGenerationStates.Normal);
    }

    public void GamePause()
    {
        fsm.ChangeState(PedestrianGenerationStates.Idle);
    }

    public void PedestrianExpired()
    {
        m_currentGeneratedNum--;
    }

    public void GeneratePedestrianRandomly(int num)
    {
        for (int i = 0; i < num; i++)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue < 0.25f)
            {
                //simple
                GenerateSimplePedestrian(1);
            }else if (randomValue < 0.5f)
            {
                //movable
                GenerateMovablePedNoProtection(1);
            }
            else if (randomValue < 0.75f)
            {
                //unmovable
                GenerateUnmovablePed(1);
            }
            else
            {
                //patient
                GeneratePatient(1);
            }
        }
    }

    public void GenerateSimplePedestrian(int num)
    {
        for (int i = 0; i < num; i++)
        {
            var ped = Instantiate(NormalPedestrians[m_rollingNumber % NormalPedestrians.Count]);
            // var pedMovingScript = ped.GetComponent<SimplePedestrian>();
            //change next

            m_rollingNumber++;
        }
    }

    public void GenerateMovablePedNoProtection(int num)
    {
        for (int i = 0; i < num; i++)
        {
            var ped = Instantiate(MovablePedestriansWithoutProtection[m_rollingNumber % MovablePedestriansWithoutProtection.Count]);
            
            m_rollingNumber++;
        }
    }

    public void GenerateUnmovablePed(int num)
    {
        for (int i = 0; i < num; i++)
        {
            var ped = Instantiate(UnMovablePedestriansWithoutProtection[m_rollingNumber % UnMovablePedestriansWithoutProtection.Count]);

            m_rollingNumber++;
        }
    }

    public void GeneratePatient(int num)
    {
        for (int i = 0; i < num; i++)
        {
            var ped = Instantiate(Patients[m_rollingNumber % Patients.Count]);

            m_rollingNumber++;
        }
    }
    
    

    #region Idle

    void Idle_Enter()
    {
        //do nothing?
    }
    
    void Idle_Update()
    {
        
    }
    
    void Idle_Exit()
    {
        
    }


    #endregion

    #region Normal

    private float _generationClock = 0.0f;
    

    void Normal_Enter()
    {
        
    }
    
    void Normal_Update()
    {
        _generationClock += Time.deltaTime;

        //产生的条件--时间到并且当前生成的个体数量要少于最多数量
        if (m_currentGeneratedNum < PedestrianNumberMax && _generationClock >= GenerateIntervalMax)
        {
            this.PedestrianNumberMax = m_initPedNumMax + Mathf.RoundToInt(Gamemanager.Instance.DifficultyLevel * 4);
            GeneratePedestrianRandomly(1);
            m_currentGeneratedNum++;
            _generationClock = 0.0f;
        }
        
    }
    
    void Normal_Exit()
    {
        
    }

    #endregion
}
