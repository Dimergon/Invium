#!/bin/sh
mdtool build --configuration:dotNET --target:Clean &
mdtool build --configuration:'dotNET64|x64' --target:Clean &
mdtool build --configuration:Mono --target:Clean &
mdtool build --configuration:'Mono64|x64' --target:Clean
mdtool build --configuration:dotNET --target:Build &
mdtool build --configuration:'dotNET64|x64' --target:Build &
mdtool build --configuration:Mono --target:Build &
mdtool build --configuration:'Mono64|x64' --target:Build
