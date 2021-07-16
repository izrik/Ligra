#!/bin/bash

# repl:
# >>> render --tokenized --namespace MetaphysicsIndustries.Ligra --to-file LigraGrammar2.cs --base SolusGrammar --using MetaphysicsIndustries.Solus --skip-imported LigraGrammar commands         
giza render --tokenized --namespace MetaphysicsIndustries.Ligra --base SolusGrammar --using MetaphysicsIndustries.Solus --skip-imported LigraGrammar LigraGrammar.giza > LigraGrammar2.cs
