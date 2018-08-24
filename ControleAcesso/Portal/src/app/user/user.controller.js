(function() {
  'use strict';

  angular
    .module('portal')
    .controller('UserController', UserController);

  /** @ngInject */

  function UserController($scope, UserService, $stateParams, $state) {
        UserService.getConfig().then(
        function(data){
            UserService.setConfig(data.data); 
        });
        $scope.user = {};
        $scope.user.ips = [];
        $scope.ipsSelected = [];
        $scope.userId = null;

        $scope.message = {
          success : false,
          error : false,
          text : ""
        }

        if($stateParams.id){
            $scope.userId = $stateParams.id;
            UserService.getUser($stateParams.id).then(function(result){
                result.data.ips = result.data.ips.split(";");
                $scope.user = result.data;

            }),(function(error){
              $state.go("home", { msgError : "Usuário não encontrado"})
           }); 
        }

        $scope.AddIP = function(ip){
            if(ip){
              if(/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ip)) { 
                  if($scope.user.ips.length < 32){
                      if(!$scope.user.ips.find(function(element){
                        return $scope.user.ips.indexOf(ip) != -1;
                      })){
                        $scope.user.ips.push(ip);
                      }else{
                        $scope.message.success = false;
                        $scope.message.error = true;
                        $scope.message.text = "O IP que tentou inserir já existe";
                        window.scrollTo(0, 0);
                      }
                      $scope.ip = "";
                  }else{
                        $scope.message.success = false;
                        $scope.message.error = true;
                        $scope.message.text = "A lista de IPs excede a quantidade permitida de IPs";
                        window.scrollTo(0, 0);
                  }
              }else{
                      $scope.message.success = false;
                      $scope.message.error = true;
                      $scope.message.text = "O IP que tentou inserir é inválido";
                      window.scrollTo(0, 0);
              }
            }else{
                    $scope.message.success = false;
                    $scope.message.error = true;
                    $scope.message.text = "Não é possível inserir um valor vázio";
                    window.scrollTo(0, 0);
            }

            
            
        }

        $scope.RemoveIPSelected = function(selected){
           $scope.user.ips = $scope.user.ips.filter(function(i){
              return selected.indexOf(i) == -1;
           });
           
        }

        $scope.SaveUser = function(){
            var userTemp =  angular.copy($scope.user);
            userTemp.ips = userTemp.ips.toString().replace(/,/g, ';');
            UserService.saveUser(userTemp).then(
            function(result){
              $state.go("home", { msgSuccess : "Gravação efetuada com sucesso!"})
            },function(error){
              $scope.message.success = false;
              $scope.message.error = true;
              $scope.message.text = error;
              window.scrollTo(0, 0);
            }); 
        }   
  }

})();
