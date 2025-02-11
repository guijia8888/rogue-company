﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Clock
{
    class Timer
    {
        public double absoluteTime = 0f;
        public int repeat = 0;
        public bool used = false;
    }

    private int currentTimerPoolIndex = 0;
    private double elapsedTime = 0f;
    private bool isInUpdate = false;
    private List<Timer> timerPool = new List<Timer>();

    private Dictionary<System.Func<BT.State>, Timer> timers = new Dictionary<System.Func<BT.State>, Timer>();
    private HashSet<System.Func<BT.State>> removeTimers = new HashSet<System.Func<BT.State>>();
    private Dictionary<System.Func<BT.State>, Timer> addTimers = new Dictionary<System.Func<BT.State>, Timer>();

    private Timer getTimerFromPool(double absoluteTime, int repeat)
    {
        int i = 0;
        int l = timerPool.Count;
        Timer timer = null;
        while (i < l)
        {
            int timerIndex = (i + currentTimerPoolIndex) % l;
            if (!timerPool[timerIndex].used)
            {
                currentTimerPoolIndex = timerIndex;
                timer = timerPool[timerIndex];
                break;
            }
            i++;
        }

        if (timer == null)
        {
            timer = new Timer();
            currentTimerPoolIndex = 0;
            timerPool.Add(timer);
        }

        timer.used = true;
        timer.absoluteTime = absoluteTime;
        timer.repeat = repeat;
        return timer;
    }
    /// <summary>
    /// UnityContext에 의해 실행되는 스케줄링 함수.
    /// 매개변수로 경과시간을 계산.
    /// </summary>
    /// <param name="deltaTime">Time.deltaTime</param>
    public void Update(float deltaTime)
    {
        this.elapsedTime += deltaTime;

        this.isInUpdate = true;

        Dictionary<System.Func<BT.State>, Timer>.KeyCollection keys = timers.Keys;

        foreach (System.Func<BT.State> timer in keys)
        {
            if (this.removeTimers.Contains(timer))
            {
                continue;
            }

            Timer time = timers[timer];
            if (time.absoluteTime <= this.elapsedTime)
            {
                if (time.repeat == 0)
                {
                    RemoveTimer(timer);
                }
                else if (time.repeat >= 0)
                {
                    time.repeat--;
                }
                timer.Invoke();
            }
        }

        foreach (System.Func<BT.State> action in this.addTimers.Keys)
        {
            if (this.timers.ContainsKey(action))
            {
                this.timers[action].used = false;
            }
            this.timers[action] = this.addTimers[action];
        }
        foreach (System.Func<BT.State> action in this.removeTimers)
        {
            timers[action].used = false;
            this.timers.Remove(action);
        }

        this.addTimers.Clear();
        this.removeTimers.Clear();

        this.isInUpdate = false;
    }
    /// <summary>
    /// 스케줄링 리스트에 행동을 저장합니다.
    /// </summary>
    /// <param name="time">반복 간격</param>
    /// <param name="repeat">반복 횟수, -1일 경우 무한</param>
    /// <param name="action">행동 대리자</param>
    public void AddTimer(float time, int repeat, System.Func<BT.State> action)
    {        
        if (!isInUpdate)
        {
            if (this.timers.ContainsKey(action))
            {
                this.timers[action].absoluteTime = elapsedTime + time;
                this.timers[action].repeat = repeat;
            }
            else
            {
                this.timers[action] = getTimerFromPool(elapsedTime + time, repeat);
            }
        }
        else
        {
            if (!this.addTimers.ContainsKey(action))
            {
                this.addTimers[action] = getTimerFromPool(elapsedTime + time, repeat);
            }
            else
            {
                this.addTimers[action].repeat = repeat;
                this.addTimers[action].absoluteTime = elapsedTime + time;
            }

            if (this.removeTimers.Contains(action))
            {
                this.removeTimers.Remove(action);
            }
        }
    }
    /// <summary>
    /// 스케줄링 리스트에 행동을 삭제합니다.
    /// </summary>
    /// <param name="action"></param>
    public void RemoveTimer(System.Func<BT.State> action)
    {
        if (!isInUpdate)
        {
            if (this.timers.ContainsKey(action))
            {
                timers[action].used = false;
                this.timers.Remove(action);
            }
        }
        else
        {
            if (this.timers.ContainsKey(action))
            {
                this.removeTimers.Add(action);
            }
            if (this.addTimers.ContainsKey(action))
            {
                this.addTimers[action].used = false;
                this.addTimers.Remove(action);
            }
        }
    }
    //TODO : 몬스터가 죽어도 스케쥴러에 메소드가 남으므로 이를 삭제하기 위한 함수, 언제 부를지, 최적화 필요
    public void RemoveAllTimer()
    {
        timers.Clear();
    }
}
