using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal
{
    public string name;
    public float value;

    public float getDiscontentment(float newValue)
    {
        return newValue * newValue;
    }

    public Goal (string goalName, float goalValue)
    {
        name = goalName;
        value = goalValue;
    }

    //public abstract float getChange();
}

public class Action
{
    public string name;

    public List<Goal> targetGoals;

    public Action (string actionName)
    {
        name = actionName;
        targetGoals = new List<Goal>();
    }

    public float getGoalChange(Goal goal)
    {
        foreach (Goal target in targetGoals)
        {
            if (target.name == goal.name)
            {
                return target.value;
            }
        }

        return 0f;
    }

    //public abstract float getDuration();
}

public class Decider : MonoBehaviour
{
    Goal[] goals;
    Action[] actions;
    Action changeOverTime;
    const float TICK_LENGTH = 5.0f;

    void Start()
    {
        // initial goals
        goals = new Goal[3];
        goals[0] = new Goal("Eat", 4);
        goals[1] = new Goal("Sleep", 3);
        goals[2] = new Goal("Bathroom", 3);

        // the long part where I define all the actions I can take
        actions = new Action[6];

        actions[0] = new Action("eat a quesadilla");
        actions[0].targetGoals.Add(new Goal("Eat", -3f));
        actions[0].targetGoals.Add(new Goal("Sleep", +2f));
        actions[0].targetGoals.Add(new Goal("Bathroom", +1f));

        actions[1] = new Action("eat an entire sleeve of oreos");
        actions[1].targetGoals.Add(new Goal("Eat", -2f));
        actions[1].targetGoals.Add(new Goal("Sleep", -1f));
        actions[1].targetGoals.Add(new Goal("Bathroom", +1f));

        actions[2] = new Action("sleep in the bed");
        actions[2].targetGoals.Add(new Goal("Eat", +2f));
        actions[2].targetGoals.Add(new Goal("Sleep", -4f));
        actions[2].targetGoals.Add(new Goal("Bathroom", +2f));

        actions[3] = new Action("sleep on the couch");
        actions[3].targetGoals.Add(new Goal("Eat", +1f));
        actions[3].targetGoals.Add(new Goal("Sleep", -2f));
        actions[3].targetGoals.Add(new Goal("Bathroom", +1f));

        actions[4] = new Action("go to the bathroom");
        actions[4].targetGoals.Add(new Goal("Eat", +4f));
        actions[4].targetGoals.Add(new Goal("Sleep", +1f));
        actions[4].targetGoals.Add(new Goal("Bathroom", +2f));

        actions[5] = new Action("drink some juice");
        actions[5].targetGoals.Add(new Goal("Eat", 0f));
        actions[5].targetGoals.Add(new Goal("Sleep", 0f));
        actions[5].targetGoals.Add(new Goal("Bathroom", -4f));

        // lets pass some time
        changeOverTime = new Action("tickTock");
        changeOverTime.targetGoals.Add(new Goal("Eat", +4f));
        changeOverTime.targetGoals.Add(new Goal("Sleep", +1f));
        changeOverTime.targetGoals.Add(new Goal("Bathroom", +2f));

        Debug.Log("Starting clock - one hour will pass every " + TICK_LENGTH
            + " seconds");
        InvokeRepeating("Tick", 0f, TICK_LENGTH);

        Debug.Log("Hit E to do something");
    }

    void Tick()
    {
        foreach (Goal goal in goals)
        {
            goal.value += changeOverTime.getGoalChange(goal);
            goal.value = Mathf.Max(goal.value, 0);
        }

        PrintGoals();
    }

    void PrintGoals()
    {
        string goalString = "";
        foreach(Goal goal in goals)
        {
            goalString += goal.name + ": " + goal.value + "; ";
        }
        goalString += "Discontentment: " + CurrentDiscontentment();
        Debug.Log(goalString);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Action bestThingToDo = ChooseAction(actions, goals);
            Debug.Log("I think I'll " + bestThingToDo.name);

            foreach (Goal goal in goals)
            {
                goal.value += bestThingToDo.getGoalChange(goal);
                goal.value = Mathf.Max(goal.value, 0);
            }

            PrintGoals();
        }
    }

    public Action ChooseAction(Action[] actions, Goal[] goals)
    {
        // find the action leading to the lowest discontentment
        Action bestAction = null;
        float bestValue = Mathf.Infinity;

        foreach (Action thisAction in actions)
        {
            float thisValue = Discontentment(thisAction, goals);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = thisAction;
            }
        }

        return bestAction;
    }

    float Discontentment(Action action, Goal[] goals)
    {
        // keep a running total
        float discontentment = 0;

        // loop through each goal
        foreach (Goal goal in goals)
        {
            // calculate the new value after the action
            float newValue = goal.value + action.getGoalChange(goal);

            // calculate the change due to time alone
            //newValue += action.getDuration() * goal.getChange();
            newValue = Mathf.Max(newValue, 0);

            // get the discontentment of this value
            discontentment += goal.getDiscontentment(newValue);
        }

        return discontentment;
    }

    float CurrentDiscontentment()
    {
        float total = 0f;
        foreach(Goal goal in goals)
        {
            total += (goal.value * goal.value);
        }

        return total;
    }
}
