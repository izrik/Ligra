using System;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Giza;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraParser : SolusParser
    {
        LigraGrammar _grammar = new LigraGrammar();
        Parser _parser;
        Spanner _numberSpanner;

        public LigraParser()
        {
            _parser = new Parser(_grammar.def_commands);
            _numberSpanner = new Spanner(_grammar.def_float_002D_number);
        }
    }
}

