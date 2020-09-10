# Wireless Sensor Networks (WSNs) Simulator

FLORA (Fuzzy based Load-Balanced Opportunistic Routing for Asynchronous Duty-Cycled WSNs)
----

The nodes close to the sink have to undertake more duties than the rest of network nodes, which imposes the nodes close to the sink to deplete their energy faster and significantly degrading the performance of network. Therefore, the nodes located in different positions should play different roles during the routing process. 
	We call this the location characteristics of nodes or the roles of nodes. FLORA achieves the location characteristics of nodes by introducing the fuzzy logic theory into WSNs. 
	To simulate FLORA, a simulator is developed based on [https://github.com/haobani/WSNSIM].

Developed by Ammar Hawbani et al.  (anmande@ustc.edu.cn), Copyright © 2019 Ammar Hawbani et al. 

Implementation 
-----
Crisps, fuzzy sets and fuzzy membership functions are implemented in:
[https://https://github.com/howbani/FLORA/tree/master/FuzzySets] 

The number of hops to the sink is computed on
[https://github.com/howbani/FLORA/blob/master/Intilization/HopsToSinkComputation.cs]

The actions of nodes are calculated on
[https://github.com/howbani/FLORA/blob/master/ControlPlane/NOS/FlowEngin/UplinkRouting.cs]

BOX-MAC is performed in the link
[https://github.com/howbani/FLORA/blob/master/Dataplane/BoXMAC.cs]
	
Node is implemented in
[https://github.com/howbani/FLORA/blob/master/Dataplane/Sensor.xaml.cs] 

Experimental Setting
-----
When the simulation starts, the nodes whose initial energy is 0.5 Jouls are randomly deployed in a 400m*400m sensing field. The sink node starting with infinite energy is positioned in the center of the field. All nodes run BOX-MAC, hava the same duty cycles and consume the energy according to the First Order Radio Model.
	In our simulations, we assume that the network has the ideal propagation condition by utilizing the Free Space Propagation Model and that the network generates a data packet from a random sensor node to the sink in each 0.1s.
	The size of the data packets is set to 1024bits. The default active time and sleep time are 2s and 1s respectively.
	
Modification 
-----
If you decide to use this simulator for academic issues, please support us by citing any of the following works:

[1] P. Liu, X. Wang, A. Hawbani, O. Busaileh, L. Zhao and A. Y. Al-Dubai, "FRCA: A Novel Flexible Routing Computing Approach for Wireless Sensor Networks," in IEEE Transactions on Mobile Computing. doi: 10.1109/TMC.2019.2928805 keywords: {Routing;Wireless sensor networks;Routing protocols;Measurement;Mobile computing;Computational modeling;wireless sensor networks;probabilistic routing;distributed routing}, URL: http://ieeexplore.ieee.org/stamp/stamp.jsp?tp=&arnumber=8766867&isnumber=4358975

[2] A. Hawbani, X. Wang, Y. A. AL-SHARABI, A. Ghannami, H. Kuhlani and S. Karmoshi, "Load-Balanced Opportunistic Routing for Asynchronous Duty-cycled WSN," in IEEE Transactions on Mobile Computing. doi: 10.1109/TMC.2018.2865485 keywords: {Routing;Measurement;Wireless sensor networks;Routing protocols;Batteries;Mobile computing;Asynchronous Duty-Cycled Routing;Load-Balanced Routing;Opportunistic Routing;Wireless Sensor Networks}, URL: http://ieeexplore.ieee.org/stamp/stamp.jsp?tp=&arnumber=8436421&isnumber=4358975

[3] A. Hawbani, X. Wang, A. Abudukelimu, H. Kuhlani, A. Qarariyah and A. Ghannami, "Zone Probabilistic Routing for Wireless Sensor Networks," in IEEE Transactions on Mobile Computing. doi: 10.1109/TMC.2018.2839746 keywords: {Routing;Probabilistic logic;Wireless sensor networks;Routing protocols;Probability distribution;Batteries;anycast routing;distributed routing;probabilistic routing;zone Routing;wireless sensor networks}, URL: http://ieeexplore.ieee.org/stamp/stamp.jsp?tp=&arnumber=8362940&isnumber=4358975

More details are explained in the link:http://staff.ustc.edu.cn/~anmande/miniflow/ or contact me via anmande@ustc.edu.cn
	

Bugs
-----
If any bugs are encountered during the execution of the toolkit, please restart toolkit, or you can download the new version of this toolkit. If there is any error occurs, the toolkit will shut down automatically.

Installation Problems
-----
1-      I can't run the Tool kit: 

        -          Install the DOT NET 4.5: Click here to download.
2-      I can't see the topologies:

     -          Toolkit can't read the topology, please install the OLEDB engine. Click here to download.
3-      If you encounter any other problems, please check your operating system capability. This toolkit is tested on windows 10 platform.

