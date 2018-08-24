(function() {
  'use strict';

  angular
    .module('portal')
    .controller('MainController', MainController);

  /** @ngInject */
  function MainController($scope,$timeout, webDevTec, toastr, UserService, $stateParams, uiGridConstants, $window) {
    $scope.highlightFilteredHeader = function( row, rowRenderIndex, col, colRenderIndex ) {
      if( col.filters[0].term ){
        return 'header-filtered';
      } else {
        return '';
      }
    };

    $scope.columns = [{ name:'Nome' ,field: 'name'},
    { field:'enabled', name: 'Ativo', type: 'boolean', cellTemplate: "<div class='ui-grid-cell-contents'>{{row.entity.enabled ? 'Sim' : 'Não'}}</div>",  
      filter: {
      type: uiGridConstants.filter.SELECT,
      selectOptions: [{value: true, label: "Sim"}, {value: false, label: "Não"}]
      }
    },
    { field:'requests', name: 'Número Requisições'}, { field:'limit', name: 'Limite'}]; 
    
    $scope.message = {
      success : false,
      error : false,
      text : ""
    }

    if($stateParams.msgError){
       $scope.message.error = true;
       $scope.message.success = false;
       $scope.message.text = $stateParams.msgError;
    }else if($stateParams.msgSuccess){
       $scope.message.error = false;
       $scope.message.success = true;
       $scope.message.text = $stateParams.msgSuccess;
    }

    $scope.gridOptions = {
        enableFiltering: true,
        columnDefs: $scope.columns,
        enableRowSelection : true,
        enableRowHeaderSelection: false,
        onRegisterApi: function(gridApi){
        $scope.gridApi = gridApi;
        $scope.gridApi.grid.registerRowsProcessor($scope.SingleFilter, 200);
    },
    }
    $scope.gridOptions.multiSelect = false;
    $scope.gridOptions.modifierKeysToMultiSelect = false;

    $scope.getAllUsers = function(){
      UserService.getAllUsers().then(function(result){
        $scope.gridOptions.data = result.data;
      }),(function(error){
        console.log(error);
      });  
    }
    $scope.FilterUser = function(){
      $scope.gridApi.grid.refresh();
    } 

    $scope.SingleFilter = function( renderableRows ){
      var matcher = new RegExp($scope.filter,"i");
      renderableRows.forEach(function(row) {
        var match = false;
        ['name'].forEach(function( field ){
          if(row.entity[field].match(matcher)){
            match = true;
          }
        });
        if (!match){
          row.visible = false;
        }
      });
      return renderableRows;
    };

    $scope.GetCurrentSelectionId = function() {
      var currentSelection = $scope.gridApi.selection.getSelectedRows();   
      if(currentSelection.length){
        $scope.currentSelection = currentSelection[0];
        return true;
      }else{
       return false;
      }
    };

    $scope.DeleteUser = function(){
      if (confirm("Deseja apagar o registro?")){     
        UserService.deleteUser($scope.currentSelection).then(function(result){
          $scope.message.success = true;
          $scope.message.error = false;
          $scope.message.text = "";
          $scope.getAllUsers();
        }),(function(error){
          $scope.message.success = false;
          $scope.message.error = true;
          $scope.message.text = "Não foi possível excluir o usuário";
        });
        }
     }
    UserService.getConfig().then(
        function(data){
            UserService.setConfig(data.data); 
            $scope.getAllUsers();
    });
    
  }
})();
