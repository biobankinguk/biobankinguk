# Deploying Elastic Search To Azure

The below guide explains how to deploy and setup an Elastic Search instance on an Azure virtual machine for the Directory. It assumes you know how to create/manage resources on Azure.

## Creating The Virtual Machine

The virtual machine should be created using [Bitnami's Elastic Search VM Image](https://azuremarketplace.microsoft.com/en-us/marketplace/apps/bitnami.elastic-search). This image is pre-configured to run an Elastic Search instance. Being certified by Bitnami, the image is guarenteed to be "always up-to-date, secure, and built to work right out of the box".


For a test environment, you most likely will want to have the instance running on the pre-configured `Standard_B1ms` instance size, with default disk options. For higher loads (greater volume of search queries and/or index sizes) you may required a higher spec instance, or a cluster of mutliple instances (not covered in this guide).

    It is highly recommended the instance is secured via an SSH key pair.

The VM will need to be connected to a Virtual Network - if one does not exist, one will be created for the Virtual Machine. The VM instance can be kept within the default subnet of the virtual network.

The virtual machine will also required a public IP address initally, so that the elastic search instance can be configured via your local machine.

On creation of the Virtual Machine, the Azure Portal will provide the SSH private key (unless the instance is password protected). This should be kept in secure location.

## Configuring Elastic Search

The elastic search indices need to be created, before it can be populated and used by the Directory. The easiest way to populate the correct indices to the elastic search instance is via the `./configure-search.ps1` Powershell script.

To be able to connect to the instance via the script, a temporary firewall rule must be created. This can be added from the Azure portal, under the Networking settings for the Virtual Machine.

| Port | Protocol | Source      | Destination |
| ---- |:--------:|:-----------:|:-----------:|
| 9200 | TCP      | `<your-ip>` | Any         | 

\
Once this rule has been applied the `configure-search.ps1` script can be ran - its basic usage is

```bash
./configure-search.ps1 -url [elastic-search-url] -dir [json config folder] <-delete> <-create>
```

The `./directory index setup` folder contains the index definitons used by the Directory. 

Since the elastic search index is brand new, we only need to use the `-create` flag when calling the script. 

```
./configure-search.ps1 -url [elastic-search-url] -dir "./directory index setup" -create
```

More information about the powershell script can be found via its dedicated [README](./README.md).

## Configuring The Virtual Network

### Virtual Machine

The virtual machine should already be connected to a virtual network at the time of creation. The idea of the virtual network is to have both the App Service and Elastic Search instance communicate with each other, withouth the need for the Elastic Search instance being connected to the Internet.

### App Service

The App Service needs to be connected to the same Virtual Network as the Elastic Search instance in order to connect to it. For an App Service, this is known as `V-Net Integration` which can be found under the Networking menu for the App Service in the Azure Portal.

**Note:** *Your App Service plan must be a Standard, Premium or Premium V2 plan to be able to use V-Net Integration. If you are not using one of these App Service plans, this guide is not for you. Instead, you will have to secure your elastic search instance via its [own authentication scheme](https://www.elastic.co/guide/en/elasticsearch/reference/current/secure-cluster.html)*

However, an App Service cannot be added to the default subnet of a virtual network. Hence, a secondary subnet must be created. A new subnet can be created under the virtual network resource.

[🔗 Microsoft Docs: Add, change, or delete a virtual network subnet](https://docs.microsoft.com/en-us/azure/virtual-network/virtual-network-manage-subnet#add-a-subnet)

Once the App Service is on the virtual network, it needs to be configured such that it uses the virtual network's internal DNS server and routes all of its traffic through the virtual network. This is done by setting the two configuration values for the App Service

```
WEBSITE_DNS_SERVER=168.63.129.16
WEBSITE_VNET_ROUTE_ALL=1
```

To test the virtual network is configured correctly, we can ping the Elastic Search instance from the App Service's console in the Azure Portal.

`tcpping <elastic-search-private-ip>:9200`

However, since the private IP address of the Virtual Machine is dynamic, it is much better to connect to the Elastic Search instance via it's internal hostname. 

The internal hostname of the virtual machine is based on the virtual machine's resource name, and is resolved by the DNS server on the virtual network.

`tcpping <elastic-search-name>.internal.cloudapp.net:9200` 

Finally, the Directory can be configured to use the Elastic Search instance by adding the following App Setting

```
ElasticSearchUrl=http://<elastic-search-name>.internal.cloudapp.net:9200`
```

## Populating The Elastic Search Instance

Populating the elastic search indices is the final configurational step. This takes the existing data from the Directory, and pushing it onto the elastic search instance to be properly indexed.

To populate the index with existing Directory data, a user with `SuperUser` privalleges can trigger a re-indexing of data via the `/SuperUser` portal on the Directory.

## Securing The Elastic Search Instance

There are many approaches to securing the Elastic Search instance and its Virtual Machine from malicious users. This guide takes the approach of securing the Elastic Search instance behind private Azure Virtual Network and Network Security Group (firewall).

Therefore, when not being configured, the temporary firewall rule and public IP address must be removed from the virtual machine. This prevents any external access (from the internet) to the elastic search instance. This is esepcially important as the elastic search instance itself has no configured authentication mechanism. 

**The elastic search instance should only be open to an external machine when it needs to be managed via its REST API (port 9200) or via an SSH session (port 22).**

## Troubleshooting

By default, the Bitnami VM image is configured with the Elastic Search instance running. This can be verified by connecting to the elastic search instance via SSH and running

`sudo /opt/bitnami/ctlscript.sh status`

If you find the instance is not running, it can be started with same script with the command

`sudo /opt/bitnami/ctlscript.sh start`

[🔗 Further documentation can be found via Bitnami](https://docs.bitnami.com/azure/apps/elasticsearch/)