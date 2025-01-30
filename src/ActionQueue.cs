using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input Buffer Class
/// </summary>
public class ActionQueue
{
    public bool IsPerformingAction { get; private set; } = false;
    private Queue<float> actionQueue = new Queue<float>();
    private float actionBufferTime;
    private Coroutine resetCoroutine;
    private MonoBehaviour owner;
    private Action action;
    public string DebugText { get; private set; } = "Ready";
    /// <summary>
    /// ActionQueue Constructor
    /// </summary>
    /// <param name="owner">owner</param>
    /// <param name="action">The desired action to trigger</param>
    /// <param name="bufferTime">buffer time </param>
    public ActionQueue(MonoBehaviour owner, Action action, float bufferTime = 0.4f)
    {
        this.owner = owner;
        this.action = action;
        this.actionBufferTime = bufferTime;
    }

    public void TryExecute()
    {
        DebugText = "Action Attempted";
        if (!IsPerformingAction)
        {
            DebugText = "Executing Action";
            ExecuteAction();
        }
        else
        {
            DebugText = "Action Queued";
            actionQueue.Enqueue(Time.time);
        }
    }

    private void ExecuteAction()
    {
        DebugText = "Action Executed";
        IsPerformingAction = true;

        if (resetCoroutine != null)
            owner.StopCoroutine(resetCoroutine);

        resetCoroutine = owner.StartCoroutine(ResetAction());
        action?.Invoke();
    }

    private IEnumerator ResetAction()
    {
        yield return new WaitForSeconds(0.6f);
        IsPerformingAction = false;
        DebugText = "Action Finished";

        while (actionQueue.Count > 0 && Time.time - actionQueue.Peek() > actionBufferTime)
        {
            DebugText = "Expired Action Request Removed";
            actionQueue.Dequeue();
        }

        if (actionQueue.Count > 0)
        {
            DebugText = "Executing Queued Action";
            actionQueue.Dequeue();
            ExecuteAction();
        }
        else
        {
            DebugText = "No Valid Queued Actions";
        }
    }
}

 
