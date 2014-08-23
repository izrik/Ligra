#!/bin/bash

git submodule init
git submodule update

pushd MetaphysicsIndustries.Acuity
./make-ready.sh
popd

