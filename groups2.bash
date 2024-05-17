#!/bin/bash
set -x
az group delete --name rg-mars --yes
az group delete --name rg-split --yes
az group delete --name rg-split-csharp --yes
az group delete --name rg-split-csharp-demo --yes
az group delete --name rg-split-demo --yes
az group delete --name rg-split-demo-glowing-capybara --yes
az group delete --name rg-split-demo-space-fiesta --yes
az group delete --name rg-split-demo-opulent-bear --yes
az group delete --name rg-split-demo-verbose-giggle --yes
az group delete --name rg-split-bear-space-waddle --yes
az group delete --name rg-split-psychic-dollop-bear --yes
az group delete --name rg-split-demo-fantastic-space --yes
az group delete --name rg-split-demo-turbo-space --yes
az group delete --name rg-split-demo-bookish-space --yes
az group delete --name dbmdemo2 --yes
az group delete --name rg-demo --yes
