Install dotnet 2.1 core on the pi


$sudo apt-get -y update
$ sudo apt-get -y install libunwind8 gettext
$ wget wget https://dotnetcli.blob.core.windows.net/dotnet/Sdk/release/2.1.6xx/dotnet-sdk-latest-linux-arm.tar.gz

$ wget https://dotnetcli.blob.core.windows.net/dotnet/aspnetcore/Runtime/2.1.0/aspnetcore-runtime-2.1.0-linux-arm.tar.gz
$ sudo mkdir /opt/dotnet
$ sudo tar -xvf dotnet-sdk-latest-linux-arm.tar.gz -C /opt/dotnet/
$ sudo tar -xvf aspnetcore-runtime-2.1.0-linux-arm.tar.gz -C /opt/dotnet/
$ sudo ln -s /opt/dotnet/dotnet /usr/local/bin


GPIO library : https://github.com/unosquare/raspberryio#digital-read-and-write

SSH deploy info to add in vsproj

<PropertyGroup>
		<SshDeployHost>192.168.0.111</SshDeployHost>
		<SshDeployClean />
		<SshDeployTargetPath>/home/pi/RaspCore</SshDeployTargetPath>
		<SshDeployUsername>pi</SshDeployUsername>
		<SshDeployPassword>raspberry</SshDeployPassword>
		<RunPostBuildEvent>OnBuildSuccess</RunPostBuildEvent>
	</PropertyGroup>
	
	<Target Condition="$(BuildingInsideSshDeploy) ==''" Name="PostBuild" AfterTargets="PostBuildEvent">
	   <Exec Command="cd $(ProjectDir)" />
	   <Exec Command="dotnet-sshdeploy push" />
	</Target>


We also need the wiring pi lib:

sudo apt-get install wiringpi