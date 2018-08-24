(function() {
  'use strict';

  angular
    .module('portal')
    .service('UserService', UserService);

    /** @ngInject */

  function UserService($http, $q) {
      var config;
      var service = { 
        getAllUsers : function(){
            return $http({
                method : "GET",
                url: config.serverURL + "users"
            })
        },
        getUser: function(id){
          return $http({
                method: "GET",
                url: config.serverURL + "users/" + id.toString()
          })
        },
        saveUser: function(user){
          var promisses = [];

          var servers = config.updateServers;

          angular.forEach(servers,function(serverUrl){
            var promisse = $http({
                method: "POST",
                url: serverUrl + "users/SaveUser",
                data: JSON.stringify(user),
                contentType: "application/json, charset=utf-8",
                traditional: true,
                datatype: "json"
            });
            promisses.push(promisse);
          });
          
          return $q.all(promisses);
        },
        deleteUser: function(user){
          var promisses = [];

          var servers = config.updateServers;

          angular.forEach(servers,function(serverUrl){
            var promisse = $http({
                method: "POST",
                url: config.serverURL + "users/DeleteUser",
                data: JSON.stringify(user),
                contentType: "application/json, charset=utf-8",
                traditional: true,
                datatype: "json"
            });
            promisses.push(promisse);
          });
          
          return $q.all(promisses);  
        },
        getConfig:function(){
           return $http.get("./assets/config.json");
        },
        setConfig:function(data){
           config = data;
        }

        
     }
     return service;
  
 
  }

})();
