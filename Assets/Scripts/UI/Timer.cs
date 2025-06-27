using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public enum TimerType { CountDown, CountUp }

    [SerializeField] private TimerType timerType = TimerType.CountDown;
    [SerializeField] private float timeRemaining = 60f;
    public float CurrentTime => timeRemaining;
    [SerializeField] private bool timerIsRunning = false;
    public bool IsRunning => timerIsRunning;
    [SerializeField] private TextMeshProUGUI timeText;
    //[SerializeField] private LevelManager levelManager;

    private void Start()
    {
        if (timerType == TimerType.CountUp)
        {
            timeRemaining = 0f;
        }
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timerType == TimerType.CountDown)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                    Debug.Log("Time has run out!");
                    timeRemaining = 0;
                    timerIsRunning = false;
                    //levelManager.CompleteLevel();
                }
            }
            else if (timerType == TimerType.CountUp)
            {
                timeRemaining += Time.deltaTime;
                DisplayTime(timeRemaining);
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timerType == TimerType.CountDown)
        {
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else if (timerType == TimerType.CountUp)
        {
            float milliseconds = (timeToDisplay - Mathf.Floor(timeToDisplay)) * 1000f; 
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            float minutes = Mathf.FloorToInt(timeToDisplay / 60) % 60;
            float hours = Mathf.FloorToInt(timeToDisplay / 3600);
            timeText.text = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", hours, minutes, seconds, Mathf.FloorToInt(milliseconds));
        }
    }

    public void StartTimer()
    {
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    public void ResetTimer(float newTime)
    {
        if (timerType == TimerType.CountDown)
        {
            timeRemaining = newTime;
        }
        else if (timerType == TimerType.CountUp)
        {
            timeRemaining = 0f;
        }
        DisplayTime(timeRemaining);
    }
}
