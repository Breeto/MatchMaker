Vagrant.configure(2) do |config|

	config.vm.define "matchmaker" do |node|
		node.vm.box = "ubuntu/trusty64"
		node.vm.network "private_network", ip: "192.168.22.10"
		node.vm.network "forwarded_port", guest: 80, host: 8000
		node.vm.synced_folder "./", "/opt/matchmaker", owner: "root", group: "root"

		#node.vm.hostname = "vagrant-#{`hostname`}"

		if Vagrant.has_plugin?("vagrant-cachier")
		    config.cache.scope = :box
		end

		node.vm.provider "virtualbox" do |vb|
			vb.memory = 4096
			vb.cpus = 2
		end

		vm_host_hostname = "#{`hostname`.split('.')[0]}" 
	end

end
