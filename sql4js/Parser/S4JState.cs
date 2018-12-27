using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JState
    {
        public List<EStateType?> AllowedStatesNames { get; set; }

        public List<S4JStateGate> Gates { get; set; }

        public S4JStateGate Gate { get; set; }

        public EStateType StateType { get; set; }

        public Int32 Priority { get; set; }

        //////////////////////////////////////////

        public Boolean IsCollection { get; set; }

        public Boolean IsValue { get; set; }

        public Boolean IsSimpleValue { get; set; }

        public Boolean IsComment { get; set; }

        public Boolean IsDelimiter { get; set; }

        public Boolean IsComa { get; set; }

        ////////////////////////////////

        public S4JState()
        {
            AllowedStatesNames = new List<EStateType?>();
            Gates = new List<S4JStateGate>();
        }

        ////////////////////////////////

        public bool IsAllowed(S4JState State)
        {
            return IsAllowed(State.StateType);
        }

        public bool IsAllowed(EStateType? StateType)
        {
            if (AllowedStatesNames.Contains(null))
                return true;
            return AllowedStatesNames.Contains(StateType);
        }

        public S4JState Clone()
        {
            S4JState item = (S4JState)this.MemberwiseClone();
            item.AllowedStatesNames = this.AllowedStatesNames.ToList();
            item.Gates = this.Gates.ToList();
            return item;
        }
    }

    public class S4JStateGate
    {
        public IList<char> Start { get; set; }

        public IList<char> End { get; set; }

        public IList<char> Inner { get; set; }

        public S4JStateGate Clone()
        {
            S4JStateGate item = (S4JStateGate)this.MemberwiseClone();
            return item;
        }
    }

    public class S4JStateStackEvent
    {
        public Int32? NewIndex
        {
            get;
            set;
        }

        public IList<Char> Chars
        {
            get;
            set;
        }

        public S4JState State
        {
            get;
            set;
        }

        public Boolean Pushed
        {
            get;
            set;
        }

        public Boolean Popped
        {
            get;
            set;
        }

        public Boolean AnyChange
        {
            get { return Pushed || Popped; }
        }
    }

    public enum EStateType
    {
        S4J,
        S4J_COMMENT,
        S4J_QUOTATION,
        S4J_ARRAY,
        S4J_SIMPLE_VALUE,
        S4J_OBJECT,
        FUNCTION,
        FUNCTION_COMMENT,
        FUNCTION_BRACKETS,

        S4J_VALUE_DELIMITER,
        S4J_COMA
    }

}


