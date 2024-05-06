(-:NewTypeCom:Newtype_Global_JSDeco:-)
<style>
	#tableSelector tr:hover {
		 background: #E2D9B2;
	}
	font {
		font-size:12px;
	}
</style>

<div class="Default" style="margin-top:5px;margin-bottom:5px;">
	查詢：
	<input type="TEXT" maxlength="33" size="30" id="FilterStr" class="Default">
	<input type="button" id="btnSearch" value="查詢">
</div>
<font class="default">
	<font color="blue">您輸入的條件共有</font>&nbsp;
	<font color="red" id="listCounter"></font>&nbsp;
	<font color="blue">筆符合資料</font>
</font>
<div id="divSelector"></div>
<script>
    var ReplaceStr = '(-:newtypefunc:Request("ReplaceStr"):-)';
	var SelectorID = '(-:newtypefunc:Request("SelectorID"):-)';
	var FilterSelectorID = '(-:newtypefunc:Request("FilterSelectorID"):-)';	//供定義查詢使用的選擇器(選用)-
	var AccountID = '(-:newtypefunc:Request("AccountID"):-)';
	var Multiple = '(-:newtypefunc:Request("Multiple"):-)';
	var DataSource = '(-:newtypefunc:Request("DataSource"):-)';
	var Url = '(-:newtypefunc:Request("Url"):-)';
	var FilterStr = '(-:newtypefunc:Request("FilterStr"):-)';
	var listField;
	var listData;
	
	/**/
	var ObjInfo = {};
	ObjInfo.ReplaceStr=ReplaceStr;
	ObjInfo.SelectorID=SelectorID;
	ObjInfo.FilterSelectorID=FilterSelectorID;
	ObjInfo.AccountID=AccountID;
	ObjInfo.Multiple=Multiple;
	ObjInfo.DataSource=DataSource;
	ObjInfo.Url=Url;
	ObjInfo.FilterStr=FilterStr;
	console.log(ObjInfo);
	
	if(FilterSelectorID == ''){	
		FilterSelectorID = SelectorID;
		$('#FilterStr').focus();
		loadData(FilterStr,FilterSelectorID);
	}
	
	
	//取得選擇器資料-
	function loadData(FilterStr, FilterSelectorID){
		//console.log(FilterStr);
		//console.log(FilterSelectorID);
		DataSource = DataSource.toLowerCase();
		var listCount = 0;
		var loadFunc = function (SelectorID, FilterStr){
		
								$.ajax({
										dataType: "json",
										async: false,
										url: 'Newtype_Selectors_GetData.aspx?t='+Math.random(),
										data: {SelectorID:FilterSelectorID ? FilterSelectorID : SelectorID, 
												 FilterStr:FilterStr,
												 AccountID:AccountID,
												 DataSource:DataSource},
										success: function(list){
											listField = list.Field;
										},
										error: function(xhr, textStatus, error) {
											alert('error\n'+
													'object   : Newtype_Selectors_GetData\n'+
													'status   : '+xhr.statusText+'\n'+
													'response : \n'+xhr.responseText);
										}
									});
		
							
								if(DataSource == 'db' ){
									$.ajax({
										dataType: "json",
										async: false,
										url: 'Newtype_Selectors_GetData.aspx?t='+Math.random(),
										data: {SelectorID:FilterSelectorID ? FilterSelectorID : SelectorID, 
												 FilterStr:FilterStr,
												 AccountID:AccountID,
												 DataSource:DataSource},
										success: function(list){
											listField = list.Field;
											listData = list.Data;
										},
										error: function(xhr, textStatus, error) {
											alert('error\n'+
													'object   : Newtype_Selectors_GetData\n'+
													'status   : '+xhr.statusText+'\n'+
													'response : \n'+xhr.responseText);
										}
									});
								}else if(DataSource == 'url'){
									$.ajax({
										dataType: "json",
										async: false,
										url: Url,
										data: {FilterStr:FilterStr,
												 AccountID:AccountID},
										success: function(Data){
											listData = Data;
										},
										error: function(xhr, textStatus, error) {
											alert('error\n'+
													'object   : Newtype_Selectors_GetData\n'+
													'status   : '+xhr.statusText+'\n'+
													'response : \n'+xhr.responseText);
										}
									});
								}else if(DataSource == 'oracle'){
									$.ajax({
										dataType: "json",
										async: false,
										url: Url,
										data: {FilterStr:FilterStr,
												 ReplaceStr,ReplaceStr,
												 AccountID:AccountID},
										success: function(Data){
											listData = Data;
										},
										error: function(xhr, textStatus, error) {
											alert('error\n'+
													'object   : Newtype_Selectors_GetData\n'+
													'status   : '+xhr.statusText+'\n'+
													'response : \n'+xhr.responseText);
										}
									});
								}								
								console.log(listField);
								console.log(listData);								
								
								//組合標題 html-
								var tr = '<tr>';
								tr += '<td bgcolor="#808080" class="AF_InverseTitle" style="width:10px;"></td>';
								$.each(listField,function(i, fieldItem){
									if(fieldItem.Visible == 1){
										tr += '<td bgcolor="#808080" class="AF_InverseTitle" '+(fieldItem.FieldWidth!=null?'style="width:'+fieldItem.FieldWidth+';"':'')+'>'+fieldItem.FieldName+'</td>';
									}
								});
								tr += '</tr>';
								
								//組合內容 html-
								var bgcolor = '';
								$.each(listData,function(i, dataItem){
									if(i<100)
									{
										if(bgcolor == '#FFFFFF'){
											bgcolor = '#EFEFEF';
										}else{
											bgcolor = '#FFFFFF';
										}

										tr += '<tr name="dataItem" bgcolor="'+bgcolor+'" dataItem="'+escapeHtml(JSON.stringify(dataItem))+'">';
										tr += '<td class="Default" style="cursor:pointer;">'+
												'	<font><input name="item" type="' + (Multiple == 1 ? 'checkbox' : 'radio') + '"></font>'+
												'</td>';
										$.each(listField,function(i, fieldItem){
											if(fieldItem.Visible == 1){
												tr += '<td class="Default" style="cursor:pointer;">'+
														'	<font>'+dataItem[fieldItem.ConfigFieldID]+'</font>'+
														'</td>';
											}
										});
										tr += '</tr>';
									}
								});
								
								$('#divSelector').append('<table id="tableSelector" border="0" cellpadding="0" cellspacing="0" width="100%">'+tr+'</table>');
								return listData.length;
							}
		
		$('#divSelector table').remove();
		//console.log(SelectorID);
		//console.log(SelectorID.toString());
		SelectorID = SelectorID.toString().split(',');
		for(var i = 0;i<SelectorID.length;i++){
			listCount += loadFunc(SelectorID[i], FilterStr);
			//console.log(listCount);
		}

		//顯示總筆數-
		$('#listCounter').text(listCount);
	}
	
	
	$('#FilterStr').keypress(function(e) {
		if(e.which == 13) {
			$('#btnSearch').click();
		}
	});

	$('#btnSearch').click(function(){
		loadData($('#FilterStr').val(), FilterSelectorID);
		$('#FilterStr').focus();
	});
	
	
	$('#divSelector').delegate('tr[name=dataItem]','click',function(){
		if(Multiple == 1){
			var checked = $(this).find('[name=item]').prop('checked');
			$(this).find('[name=item]').prop('checked', !checked);
		}else{
			$(this).find('[name=item]').prop('checked', true);
		}
		//options.func(selectorDialog, data);
	});
	
	$('#divSelector').on('dblclick', 'tr[name=dataItem]', function(e) {
		  //code here
		 var data=JSON.parse($(this).attr("dataItem"));
		 console.log(data);
	 });
	
	
	function getData(){
		var item = $('[name=item]:checked');
		if(Multiple == 1){
			var data = [];
			$(item).each(function(i, o){
				data.push(JSON.parse($(o).parents('tr[name=dataItem]:first').attr('dataitem')));
			});
			return data;
		}else{
			if($(item).length == 0){
				return null;
			}else{
				return JSON.parse($(item).parents('tr[name=dataItem]:first').attr('dataitem'));
			}		
		}
	}
	//清空用Json
	function getEmptyData(){
		var JsonText=$('tr[name=dataItem]:first').attr('dataitem');
		var JsonObj=JSON.parse(JsonText);
		
		Object.keys(JsonObj).forEach(key => JsonObj[key]= '');
		console.log(JsonObj);
		return JsonObj;
	}
	function escapeHtml(text) {
		return text
					.replace(/&/g, "&amp;")
					.replace(/"/g, "&quot;")
					.replace(/'/g, "'")
					.replace(/</g, "&lt;")
					.replace(/>/g, "&gt;")
	}
</script>