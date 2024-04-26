az role assignment create --role contributor --scope /subscriptions/def76464-cef6-4c3b-9728-8952072734eb/resourceGroups/TE-DEVOps-RG --subscription def76464-cef6-4c3b-9728-8952072734eb --assignee-object-id 853fcc1a-af9e-4e09-8aca-f74242b3ee68 --assignee-principal-type ServicePrincipal

az ad app federated-credential create --id 853fcc1a-af9e-4e09-8aca-f74242b3ee68 --parameters credentials.json

az ad sp create-for-rbac --name "TechExcelDotNetDeploy" --json-auth --role contributor --scopes /subscriptions/def76464-cef6-4c3b-9728-8952072734eb

az deployment group create --resource-group te-devops-rg --template-file ./main.bicep