using System;
using System.Collections.Generic;

namespace MaquinaDeEstados
{
    public enum ProcessState 
    { 
        Idle, Walking, Running, Jumping
    }
    
    public enum Command 
    { 
        Walk, Run, Jump, Stop
    }
    
    public class Process 
    {
        class StateTransition 
        {
            readonly ProcessState CurrentState; 
            readonly Command Command; 
            public StateTransition(ProcessState currentState, Command command) 
            { 
                CurrentState = currentState; 
                Command = command; 
            } 
            public override int GetHashCode() 
            { 
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            } 
            public override bool Equals(object obj) 
            { 
                StateTransition other = obj as StateTransition; 
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command; 
            }
        } 
        Dictionary<StateTransition, ProcessState> transitions;
 
        public ProcessState CurrentState 
        { 
            get; 
            private set; 
        } 
        
        public Process() 
        { 
            CurrentState = ProcessState.Idle;
            transitions = new Dictionary<StateTransition, ProcessState> 
            {
                { new StateTransition(ProcessState.Idle, Command.Walk), ProcessState.Walking },
                { new StateTransition(ProcessState.Idle, Command.Run), ProcessState.Running }, 
                { new StateTransition(ProcessState.Idle, Command.Jump), ProcessState.Jumping }, 
                { new StateTransition(ProcessState.Running, Command.Jump), ProcessState.Jumping }, 
                { new StateTransition(ProcessState.Running, Command.Walk), ProcessState.Walking },
                { new StateTransition(ProcessState.Running, Command.Stop), ProcessState.Idle },
                { new StateTransition(ProcessState.Walking, Command.Stop), ProcessState.Idle },
                { new StateTransition(ProcessState.Walking, Command.Jump), ProcessState.Jumping },
                { new StateTransition(ProcessState.Walking, Command.Run), ProcessState.Running },
                { new StateTransition(ProcessState.Jumping, Command.Stop), ProcessState.Idle }            
            }; 
        } 
        
        public ProcessState GetNext(Command command) 
        { 
            StateTransition transition = new StateTransition(CurrentState, command); 
            ProcessState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
            {
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
            }
            return nextState; 
        } 

        public ProcessState MoveNext(Command command) 
        { 
            CurrentState = GetNext(command); return CurrentState; 
        }
    }
    
    public class Program 
    { 
        static void Main(string[] args) 
        { 
            Process p = new Process(); 
            Console.WriteLine("Current State = " + p.CurrentState); 
            Console.WriteLine("Command.Begin: Current State = " + p.MoveNext(Command.Walk)); 
            Console.WriteLine("Command.Pause: Current State = " + p.MoveNext(Command.Run)); 
            Console.WriteLine("Command.End: Current State = " + p.MoveNext(Command.Jump)); 
            Console.WriteLine("Command.Exit: Current State = " + p.MoveNext(Command.Stop)); 
            Console.ReadLine(); 
        } 
    }
}
