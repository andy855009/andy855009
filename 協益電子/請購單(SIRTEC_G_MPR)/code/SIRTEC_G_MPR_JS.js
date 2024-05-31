var bpm;
$(function () {
	bpm = $bpm({
		viewSetting: [{
			selector: "[name=Applicant]",
			visible: true,
			editing: true
		}]
	});

	bpm.detail.setTemplate({ detailId: 'detail', target: '[name=detailItem]' });

	//<D表新增一列後 執行...>
	bpm.detail.handlerAddItemEnd(function (detailId, itemElement) {
		//<更新項次>
		loadDetailNo();
		//<明細表事件>
		loadDetailEvent();
	});

	//<表單送出事件>
	bpm.handlerSend(function (actionInfo) {
		//console.log(actionInfo);
		if (bpm.formInfo.processID == "applicant") {
			//<申請表單>
			if (actionInfo.action == "submit") {
				//<送出表單>
				var checkMsg = checkRequired();
				var checkDetailMsg = checkDetailRequired();
				if (checkMsg.length > 0) {
					var tmp = checkMsg.join('\n');
					alert(tmp);
					return false;
				} else {
					if (checkDetailMsg.length > 0) {
						var tmp = checkDetailMsg.join('\n');
						alert(tmp);
						return false;
					} else {
						insertDetailData();
					}
				}
			}
			else {
				//<申請表單 - 儲存草稿>
				insertDetailData();
			}
			return true;
		} else {
			//<簽核表單>
			if (actionInfo.action == "agree") {
				//<同意>
				var log = "";
				var ProcessID = $('[name=ProcessID]').val();
				if (ProcessID == "DsntList02") {
					var checkMsg = checkPOContentRequired();
					if (checkMsg.length > 0) {
						var tmp = checkMsg.join('\n');
						alert(tmp)
						return false;
					} else {
						insertDetailData_POContent();
						updateMDetailData();
					}
				} else if (ProcessID == 'AplSlf02') {
					var checkMsg = checkRequired();
					var checkDetailMsg = checkDetailRequired();
					if (checkMsg.length > 0) {
						var tmp = checkMsg.join('\n');
						alert(tmp);
						return false;
					} else {
						if (checkDetailMsg.length > 0) {
							var tmp = checkDetailMsg.join('\n');
							alert(tmp);
							return false;
						} else {
							insertDetailData();
							updateMDetailData();
						}
					}
				}
				return true;
			} else if (actionInfo.action == "reject") {
				//簽核表單 - 駁回>

			} else {

			}
		}
		return true;
	});

	//<版面控制>
	if (bpm.formInfo.processID == "applicant") {
		//<申請畫面>
		APPLICANTLayout();
	}
	else if (bpm.formInfo.processID == "Content") {
		//<檢視畫面>
		COMMENTLayout();


		var ProcessID = $('[name=ProcessID]').val();
		if (ProcessID == 'DsntList02') {
			//HideAmountCloumn('TOTAL_AMOUNT');
			$('#applicant-po').hide();
		}

		$('[name=PO_MEMBERID]').hide();
		hideEdit($(this).find('[name=PO_MEMBERNAME]'));

		$('[name=MutiSign]').hide();
		hideEdit($(this).find('[name=MutiSign_name]'));


		//表頭-請購單


		hideEdit($(this).find('[name=EXPENSE_CATEGORY]'));
		var RadioObj = ["Priority"];
		$.each(RadioObj, function (index, value) {
			$('[name=' + value + ']:checked').removeAttr("disabled");
		});

		var AmountObj = ["BUDGET_SURPLUS", "TOTAL_AMOUNT"];
		$.each(AmountObj, function (index, value) {
			HideAmountCloumn(value);
		});

		$('[name=EXPENSE_CATEGORY]').parent().css("text-align", "left");
		$('[name=PO_MEMBERNAME]').parent().css("text-align", "left");
		//$('[name=]').parent().css("text-align", "right");
		$('[name=TOTAL_AMOUNT]').parent().css("text-align", "right");

		//表身-請購單

		$('#add').hide();
		$('#del').hide();
		$('#detail').find('thead tr th:first').hide();
		$('#detail').find('tbody th').hide();
		getDetailDB();

		$('[name=detailItem][detailtype=item]').each(function (i) {
			hideEdit($(this).find('[name=Product_Category]'));
			hideEdit($(this).find('[name=Product_Name]'));
			hideEdit($(this).find('[name=Product_Specification]'));
			hideEdit($(this).find('[name=Product_Quantity]'));
			hideEdit($(this).find('[name=Required_Delivery_Date]'));
			hideEdit($(this).find('[name=Instructions_For_Use]'));
			hideEdit($(this).find('[name=PR_DeptID]'));
			hideEdit($(this).find('[name=PR_DeptNAME]'));
			$(this).find('[name=PR_Dept_button]').hide();
			HideAmountCloumn_Detail($(this).find('[name=Pre_Budget]'));

			$(this).find('[name=Product_Category]').parent().css("text-align", "left");
			$(this).find('[name=Product_Name]').parent().css("text-align", "left");
			$(this).find('[name=Product_Specification]').parent().css("text-align", "left");
			$(this).find('[name=Product_Quantity]').parent().css("text-align", "right");
			$(this).find('[name=Required_Delivery_Date]').parent().css("text-align", "left");
			$(this).find('[name=Instructions_For_Use]').parent().css("text-align", "left");
			$(this).find('[name=Pre_Budget]').parent().css("text-align", "right");
			$(this).find('[name=PR_DeptID]').parent().css("text-align", "left");
			$(this).find('[name=PR_DeptNAME]').parent().css("text-align", "left");
		});
	} else {
		//<簽核>
		COMMENTLayout();
		$().ready(function () {
			if (ProcessID == "DsntList02") {
				$('[name=reject]').attr('onclick', 'alert("點選駁回後將無法在對現有表單做任何操作");ApproveSBTN("reject","2","Result","駁回")');
			}

			if (ProcessID != 'AplSlf02') {
				$('[name=PO_MEMBERID]').hide();
				hideEdit($(this).find('[name=PO_MEMBERNAME]'));

				$('[name=MutiSign]').hide();
				hideEdit($(this).find('[name=MutiSign_name]'));

				//表頭-請購單


				hideEdit($(this).find('[name=EXPENSE_CATEGORY]'));
				var RadioObj = ["Priority"];
				$.each(RadioObj, function (index, value) {
					$('[name=' + value + ']:checked').removeAttr("disabled");
				});

				var AmountObj = ["BUDGET_SURPLUS", "TOTAL_AMOUNT"];
				$.each(AmountObj, function (index, value) {
					HideAmountCloumn(value);
				});

				//BUDGET_SURPLUS
				//HideAmountCloumn('BUDGET_SURPLUS');

				//TOTAL_AMOUNTR
				if (ProcessID == 'DsntList02') {
					$('[name=TOTAL_AMOUNT]').attr("type", "number");
					$('[name=TOTAL_AMOUNT]').css("text-align", "right");
				} else {
					HideAmountCloumn('TOTAL_AMOUNT');
				}


				$('[name=EXPENSE_CATEGORY]').parent().css("text-align", "left");
				$('[name=PO_MEMBERNAME]').parent().css("text-align", "left");
				$('[name=BUDGET_SURPLUS]').parent().css("text-align", "right");
				$('[name=TOTAL_AMOUNT]').parent().css("text-align", "right");

				//表身-請購單


				$('#add').hide();
				$('#del').hide();
				$('#detail').find('thead tr th:first').hide();
				$('#detail').find('tbody th').hide();
				getDetailDB();
				$('[name=detailItem][detailtype=item]').each(function (i) {
					//console.log($(this));
					//hideEdit($(this).find('[name=Num]'));
					hideEdit($(this).find('[name=Product_Category]'));
					hideEdit($(this).find('[name=Product_Name]'));
					hideEdit($(this).find('[name=Product_Specification]'));
					hideEdit($(this).find('[name=Product_Quantity]'));
					hideEdit($(this).find('[name=Required_Delivery_Date]'));
					hideEdit($(this).find('[name=Instructions_For_Use]'));
					//hideEdit($(this).find('[name=Pre_Budget]'));
					HideAmountCloumn_Detail($(this).find('[name=Pre_Budget]'));
					hideEdit($(this).find('[name=PR_DeptNAME]'));
					$(this).find('[name=PR_Dept_button]').hide();



					$(this).find('[name=Product_Category]').parent().css("text-align", "left");
					$(this).find('[name=Product_Name]').parent().css("text-align", "left");
					$(this).find('[name=Product_Specification]').parent().css("text-align", "left");
					$(this).find('[name=Product_Quantity]').parent().css("text-align", "right");
					$(this).find('[name=Required_Delivery_Date]').parent().css("text-align", "left");
					$(this).find('[name=Instructions_For_Use]').parent().css("text-align", "left");
					$(this).find('[name=Pre_Budget]').parent().css("text-align", "right");
					//$(this).find('[name=PR_DeptID]').parent().css("text-align", "left");
					$(this).find('[name=PR_DeptNAME]').parent().css("text-align", "left");
				});
			}
			else //<表頭 - 請購單開啟編輯>
			{
				//<表頭 - 請購單input打開編輯
				$('#applicant-pr').find('input').each(function () {
					//console.log(this);
					var name = $(this).attr('name');
					//console.log(name);
					if (name != 'BUDGET_SURPLUS' && name != 'TOTAL_AMOUNT' && name != 'MutiSign_name' && name != 'PR_DeptNAME') {
						$(this).removeAttr("readonly");
						$(this).removeAttr("disabled");
					}
				});

				//<表頭 - 費用類型>
				//EXPENSE_CATEGORY
				//:维修     2：增购       3：软体服务费       4：代利盟支付						
				$('[name=EXPENSE_CATEGORY]').hide();
				showEdit($('[name=EXPENSE_CATEGORY_COMMENT]'));
				$('[name=EXPENSE_CATEGORY]').attr('readonly', 'readonly');
				var CATEGORYObj = ["維修", "增購", "軟體服務費", "代利盟支付"];
				$.each(CATEGORYObj, function (index, value) {
					$('[name=EXPENSE_CATEGORY]').parent().append('<input type="radio" name="EXPENSE_CATEGORY_radio" value="' + value + '">' + value);
				});
				//$('[name=EXPENSE_CATEGORY]').parent().append('<input type="text" name="EXPENSE_CATEGORY_COMMENT" hidden>');
				$('[name=EXPENSE_CATEGORY_radio]').on('click', function () {
					var EXPENSE_CATEGORY_radio = $('[name=EXPENSE_CATEGORY_radio]:checked').val();
					$('[name=EXPENSE_CATEGORY]').val(EXPENSE_CATEGORY_radio);
					if (EXPENSE_CATEGORY_radio == '代利盟支付') {
						$('[name=EXPENSE_CATEGORY_COMMENT]').show();
					} else {
						$('[name=EXPENSE_CATEGORY_COMMENT]').hide();
						$('[name=EXPENSE_CATEGORY_COMMENT]').val('');
					}

				});
				var EXPENSE_CATEGORY = $('[name=EXPENSE_CATEGORY]').val();
				//console.log(EXPENSE_CATEGORY);
				$('[name=EXPENSE_CATEGORY_radio]').each(function () {
					var value = $(this).val();
					if (EXPENSE_CATEGORY == value) {
						$(this).attr('checked', 'checked');
					}
				});

				$('[name=BUDGET_SURPLUS]').css('text-align', 'right');
				$('[name=TOTAL_AMOUNT]').css('text-align', 'right');


				$('[name=PO_MEMBERID]').hide();
				$('[name=PO_MEMBERNAME]').hide();
				var PO_MEMBERID = $('[name=PO_MEMBERID]').val();
				var PO_MEMBERNAME = $('[name=PO_MEMBERNAME]').val();
				$.ajax({
					type: "POST",
					url: "SIRTEC_G_MPR_GetSelect_POMember.aspx",
					data: {
						Company_code: $('[name=company_code]').val(),
						ApplicantID: $('[name=ApplicantID]').val(),
						ApplicantDept: $('[name=ApplicantDept]').val(),
					},
					async: false,
					dataType: "text",
					success: function (text) {
						//console.log(text);
						$('[name=PO_MEMBERNAME]').parent().append(text);
						$('[name=POMember_SelectItem]').css("width", "80%");
						$('[name=POMember_SelectItem]').on('change', function () {
							var COMBI_ID = $(this).find(":selected").attr("values");
							var COMBI_NAME = $(this).find(":selected").val();
							if (COMBI_NAME == "請選擇") {
								$('[name=PO_MEMBERID]').val('');
								$('[name=PO_MEMBERNAME]').val('');
							} else {
								$('[name=PO_MEMBERID]').val(COMBI_ID);
								$('[name=PO_MEMBERNAME]').val(COMBI_NAME);
							}
						})
					}, error: function (e) {
						console.log("Error-DisplayIdentityName");
					}
				});

				if (PO_MEMBERID != '' && PO_MEMBERNAME != '') {
					$('[name=POMember_SelectItem]').val(PO_MEMBERNAME);
				}

				$('[name=MutiSign]').removeAttr("readonly", "readonly");
				$('[name=MutiSign]').removeAttr("disabled", "disabled");
				//<表頭 - 會簽人員>
				$('[name=MutiSign]').on('click', function () {
					var TmpName = $(this).attr("name");
					//console.log(TmpName);
					//console.log(TmpName + '_name');
					//console.log($("[name=" + TmpName + "_name]").val());
					var obj = {};

					if ($("[name=" + TmpName + "_name]").val() != "") {
						var objArr = [];
						var DisplayLookArr = $("[name=" + TmpName + "_name]").val().split(",");
						var HideArr = $("[name=" + TmpName + "_id]").val().split(",");
						for (i = 0; i < DisplayLookArr.length; i++) {
							obj.部門名稱 = DisplayLookArr[i].split("_").pop();
							obj.姓名 = DisplayLookArr[i].split("_").shift();
							obj.帳號 = HideArr[i].split(",").shift();
							//obj.姓名 = HideArr[i].split("@").pop();
							//obj.部門名稱 = "100-資訊課";
							//obj.職稱 = "工程師";
							//obj.帳號 = "003121";
							//obj.姓名 = "劉俊成";
							objArr.push(obj);
							obj = {};
						}
						obj = {
							DisplayLookArr: $("[name=" + TmpName + "_name]").val().replace(/→/g, ","),
							JsonArr: objArr,
						}
					}
					//console.log(obj);

					openMultSelector({
						title: '請選擇對象',
						width: 1000,
						selectorID: 'SIRTEC_G_COMMUNICATION_InformMember',
						//ReplaceStr: $('[name=ApplicantDept]').val(),
						//ReplaceInStr: '',
						//url: 'SIRTEC_G_COMMUNICATION_GetCCMember.ASPX?DeptID=' + $('[name=receive_dept]').val(),
						//ApproverDept: $('[name=cc_dept]').val(),
						//ApproverID: $('[name=cc_id]').val(),
						UpDownBtn: 1,
						dataSource: 'Url',
						JsonArr: JSON.stringify(obj),
						func: function (selectorDialog, data) {
							var FuncAssistStr = "";
							var UnitCodeStr = "";

							var cc_dept = "";
							var cc_deptname = "";


							$.each(data, function (i, dataItem) {
								//console.log(data);
								if (FuncAssistStr == "") {
									FuncAssistStr += dataItem['部門名稱'] + "_" + dataItem['姓名'];
									UnitCodeStr += dataItem['帳號'];
								}
								else {
									FuncAssistStr += "," + dataItem['部門名稱'] + "_" + dataItem['姓名'];
									UnitCodeStr += "," + dataItem['帳號'];
								}

							});
							$("[name=" + TmpName + "_id]").val(UnitCodeStr);
							$("[name=" + TmpName + "_name]").val(FuncAssistStr);

							//$('[name=receive_dept]').val(cc_dept);
							//$('[name=receive_deptname]').val(cc_deptname);


							selectorDialog.dialog('close');
						}
					});
				});

				$('#detail').each(function (i) {
					//console.log(i);
					//console.log(this);
					$(this).find('tbody tr').each(function (i) {
						$(this).find('[name=Product_Quantity]').attr('type', 'number');
						$(this).find('[name=Product_Quantity]').css('text-align', 'right');
						$(this).find('[name=Required_Delivery_Date]').attr('type', 'date');
						$(this).find('[name=Pre_Budget]').attr('type', 'number');
						$(this).find('[name=Pre_Budget]').css('text-align', 'right');
					});
				});

				getDetailDB();
				//<表身 - 類別>
				$('[name=detailItem]').find('td input[name=Product_Category]').each(function () {
					$(this).attr('readonly', 'readonly');
					var CATEGORYObj = ["筆記型電腦", "桌上型電腦", "網通設備", "其他"];
					$(this).parent().append('<select name="Product_Category_select" class="default" style="width:100%;"></select>');
					$(this).parent().find('select').on('change', function () {
						console.log($(this).val());
					});
					$(this).parent().find('[name=Product_Category_select]').append('<option value="">請選擇</option>');
					$.each(CATEGORYObj, function (index, value) {
						$('[name=Product_Category_select]').append('<option value="' + value + '">' + value + '</option>');
					});
					$(this).parent().find('[name=Product_Category_select]').val($(this).val());
				});

				$('#itemBoxAll').removeAttr('readonly', 'readonly');
				$('#itemBoxAll').removeAttr('disabled', 'disabled');


				//<表身 - 請購單事件功能>
				$('#itemBoxAll').on('click', function () {
					$('[name=itemBox]').prop('checked', $(this).prop('checked'));
				});

				$('#add').on('click', function () {
					bpm.detail.addItem('detail');
				});

				$('#del').on('click', function () {
					$('[name=itemBox]:checked').each(function () {
						bpm.detail.delItem(this);
						loadDetailNo();
					});
				});

			}
		});
	}


});

/** 表頭卡控 */
function checkRequired() {
	var errMsgArr = [];
	var checkMessage1 = '{field} 不可空白';
	var fieldArr = [];

	//text
	var textObj = [];
	$.each(textObj, function (i, name) {
		if ($('[name=' + name + ']').val() == '') {
			var display = $('[name=' + name + ']').attr('display');
			fieldArr.push(display);
		}
	});

	//Select
	var SelectObj = [];
	SelectObj.push('PO_MEMBERID');
	$.each(SelectObj, function (i, name) {
		if ($('[name=' + name + ']').val() == '') {
			var display = $('[name=' + name + ']').attr('display');
			fieldArr.push(display);
		}
	});

	//Radio
	var RadioObj = [];
	RadioObj.push('EXPENSE_CATEGORY');
	$.each(RadioObj, function (i, name) {
		if ($('[name=' + name + ']').val() == '') {
			var display = $('[name=' + name + ']').attr('display');
			fieldArr.push(display);
		}
	});
	console.log(fieldArr.length)
	if (fieldArr.length > 0) {
		errMsgArr.push(checkMessage1.replace(/{field}/g, fieldArr.join(',')));
	}
	return errMsgArr;

}

/** 表身卡控*/
function checkDetailRequired() {
	var errMsgArr = [];
	var fieldArr = []
	var checkMessage1 = '第{row}項{field} 不可空白';
	$('[name=detailItem][detailtype=item]').each(function (i) {
		var tr = $(this);
		var ConlumnObj = ['Product_Category', 'Product_Name', 'Product_Name', 'Product_Quantity', 'Required_Delivery_Date', 'Required_Delivery_Date', 'Required_Delivery_Date', 'PR_DeptID'];
		$.each(ConlumnObj, function (j, name) {
			if (tr.find('[name=' + name + ']').val() == '') {
				var display = $('[name=' + name + ']').attr('display');
				fieldArr.push(display);
			}
		});
		if (fieldArr.length > 0) {
			var tmpMsg = checkMessage1.replace(/{row}/g, (i + 1).toString()).replace(/{field}/g, fieldArr.join(','));
			errMsgArr.push(tmpMsg);
		}
	});

	return errMsgArr;
}

/** 採購單卡控*/
function checkPOContentRequired() {
	var errMsgArr = [];
	var checkMessage1 = '比價資料 {Product_Name} 沒有選取比價結果'
	var checkMessage2 = '比價資料 {Product_Name} 序 {row} {field} 不可空白\n(不填寫未稅單價寫0)';
	var checkMessage3 = '比價資料 {Product_Name} 選取的總價不是最低，請填寫原因'

	$('[name=POContentInfo]').each(function () {
		var tbody = $(this);
		var Product_Name = '';
		var Product_Specification = '';
		tbody.find('[name=POContent_header]').each(function () {
			var head = $(this);
			Product_Name = head.find('[name=POContent_Product_Name]').val();
			Product_Specification = head.find('[name=POContent_Product_Specification]').val();
		});

		var TotalAmountAry = [];
		tbody.find('[name=POContent_table] tbody').each(function (i) {
			if ($(this).find('[type=radio]:checked').length == 0) {
				var tmpMsg = checkMessage1.replace(/{Product_Name}/g, Product_Name)
				errMsgArr.push(tmpMsg);
			} else {
				var checkTotalAmount = '';
				var checkComment = '';
				$(this).find('[type=radio]:checked').each(function () {
					checkTotalAmount = $(this).find('tr').find('[name=checkTotalAmount]').val();
					console.log(checkTotalAmount);
					comment = $(this).find('tr').find('[name=comment]').val();
				});

				$(this).find('tr').each(function () {
					var tr = $(this);
					var ItemPrice = tr.find('[name=ItemPrice]').val();
					var TotalAmount = tr.find('[name=TotalAmount]').val();
					if (ItemPrice != '0') {
						var ColumnObj = ['Customer', 'ItemSpec'];
						var field = '';
						//廠商、規格必填

						var tMsgArr1 = [];
						$.each(ColumnObj, function (i, ColumnName) {
							var name = ColumnName;
							var value = tr.find('[name=' + name + ']').val();
							if (value == '') {
								tMsgArr1.push(name);
							}
						});

						if (tMsgArr1.length > 0) {
							var tmpMsg = checkMessage2.replace(/{Product_Name}/g, Product_Name).replace(/{row}/g, (i + 1).toString()).replace(/{field}/g, tMsgArr1.join(',')) + '\n';
							errMsgArr.push(tmpMsg);
						} else {
							TotalAmountAry.push(parseFloat(TotalAmount));
						}
					}
				});
				var MinAmount = Math.min.apply(null, TotalAmountAry);
				if (checkTotalAmount != MinAmount && comment == '') {
					var tmpMsg = checkMessage3.replace(/{Product_Name}/g, Product_Name);
					errMsgArr.push(tmpMsg);
				}
			}
		});
	});
	return errMsgArr;
}


/**申請畫面 */
function APPLICANTLayout() {
	$().ready(function () {
		//<申請初始設定>
		init();

		$('[name=FORM_ID]').hide();
		//<表頭 - 部門轉換對應權限>
		$('#OPTION_COMPANY_PREFIX_CODE').empty();
		$('#OPTION_COMPANY_PREFIX_CODE').append("<SPAN><SELECT NAME=\"COMPANY_PREFIX_CODE\"\" class=\"Default\"></SELECT></SPAN>");
		SearchCompanyName();
		CompanyNameDisplay();
		$("[name=ApplicantDept]").change(function () {
			SearchCompanyName();
			CompanyNameDisplay();
			$('[name=PO_MEMBERID]').val('')
			$('[name=PO_MEMBERNAME]').val('');
			ChangeSelectItem();
		});

		$("[name=COMPANY_PREFIX_CODE]").change(function () {
			CompanyNameDisplay();
			$('[name=PO_MEMBERID]').val('')
			$('[name=PO_MEMBERNAME]').val('');
			ChangeSelectItem();
		});

		//<表身 - 類別>
		$('[name=detailItem][detailtype=template]').each(function () {
			$(this).find('[name=Product_Category]').closest('td').each(function () {
				$(this).append('<select></select>');
				$(this).find('select').each(function () {
					var Select = $(this);
					Select.attr('name', 'Product_Category_select');
					Select.attr('class', 'default');
					Select.append(new Option('請選擇', ''))
					var CATEGORYObj = ["筆記型電腦", "桌上型電腦", "網通設備", "其他"];
					$.each(CATEGORYObj, function (i, value) {
						Select.append(new Option(value, value));
					});
				});
			});
			bpm.detail.addItem('detail');
		});


		//<表身 - 請購單明細表功能>
		$('#itemBoxAll').on('click', function () {
			$('[name=itemBox]').prop('checked', $(this).prop('checked'));
		});

		//<表身 - 明細行新增>
		$('#add').on('click', function () {
			bpm.detail.addItem('detail');
		});

		//<表身 - 明細行刪除>
		$('#del').on('click', function () {
			$('[name=itemBox]:checked').each(function () {
				bpm.detail.delItem(this);
				loadDetailNo();
			});
			var count = $('[detailid=detail][dstailtype=item]').length;
			if (count == 0) {
				bpm.detail.addItem('detail');
			}
		});

		//<表頭- CSS>
		var AmountObj = ["BUDGET_SURPLUS", "TOTAL_AMOUNT"];
		$.each(AmountObj, function (index, value) {
			$('[name=' + value + ']').attr('readonly', 'readonly');
			var number = parseFloat($('[name=' + value + ']').val());
			if (isNaN(number)) {
				$('[name=' + value + ']').val(0);
			}
			$('[name=' + value + ']').css('text-align', 'right');
		});

		//<表頭 - 費用類型>
		//EXPENSE_CATEGORY
		//:维修     2：增购       3：软体服务费       4：代利盟支付
		var CATEGORYObj = ["維修", "增購", "軟體服務費", "代利盟支付"];
		$('[name=EXPENSE_CATEGORY]').hide();
		//$('[name=EXPENSE_CATEGORY]').parent().append('<br>');
		$('[name=EXPENSE_CATEGORY]').attr('readonly', 'readonly');

		$.each(CATEGORYObj, function (index, value) {
			$('[name=EXPENSE_CATEGORY]').parent().append('<input type="radio" name="EXPENSE_CATEGORY_radio" value="' + value + '">' + value);
		});
		//$('[name=EXPENSE_CATEGORY]').parent().append('<input type="text" name="EXPENSE_CATEGORY_COMMENT" hidden>');
		$('[name=EXPENSE_CATEGORY_radio]').on('click', function () {
			var EXPENSE_CATEGORY_radio = $('[name=EXPENSE_CATEGORY_radio]:checked').val();
			$('[name=EXPENSE_CATEGORY]').val(EXPENSE_CATEGORY_radio);
			if (EXPENSE_CATEGORY_radio == '代利盟支付') {
				$('[name=EXPENSE_CATEGORY_COMMENT]').show();
			} else {
				$('[name=EXPENSE_CATEGORY_COMMENT]').hide();
			}

		});


		//<表頭 - 採購人員>
		$('[name=PO_MEMBERID]').hide();
		$('[name=PO_MEMBERNAME]').hide();
		var PO_MEMBERID = $('[name=PO_MEMBERID]').val();
		var PO_MEMBERNAME = $('[name=PO_MEMBERNAME]').val();
		$.ajax({
			type: "GET",
			url: "SIRTEC_G_MPR_GetSelect_POMember.aspx",
			data: {
				Company_code: $('[name=company_code]').val(),
				ApplicantID: $('[name=ApplicantID]').val(),
				ApplicantDept: $('[name=ApplicantDept]').val(),
			},
			async: false,
			dataType: "text",
			success: function (text) {
				//console.log(text);
				$('[name=PO_MEMBERNAME]').parent().append(text);
				$('[name=POMember_SelectItem]').css("width", "80%");
				$('[name=POMember_SelectItem]').on('change', function () {
					var COMBI_ID = $(this).find(":selected").attr("values");
					var COMBI_NAME = $(this).find(":selected").val();
					if (COMBI_NAME == "請選擇") {
						$('[name=PO_MEMBERID]').val('');
						$('[name=PO_MEMBERNAME]').val('');
					} else {
						$('[name=PO_MEMBERID]').val(COMBI_ID);
						$('[name=PO_MEMBERNAME]').val(COMBI_NAME);
					}
				})
			}, error: function (e) {
				console.log("Error-DisplayIdentityName");
			}
		});


		if (PO_MEMBERID != '' && PO_MEMBERNAME != '') {
			$('[name=POMember_SelectItem]').val(PO_MEMBERNAME);
		}

		//<表頭 - 會簽人員>
		$('[name=MutiSign]').on('click', function () {
			var TmpName = $(this).attr("name");
			//console.log(TmpName);
			//console.log(TmpName + '_name');
			//console.log($("[name=" + TmpName + "_name]").val());
			var obj = {};

			if ($("[name=" + TmpName + "_name]").val() != "") {
				var objArr = [];
				var DisplayLookArr = $("[name=" + TmpName + "_name]").val().split(",");
				var HideArr = $("[name=" + TmpName + "_id]").val().split(",");
				for (i = 0; i < DisplayLookArr.length; i++) {
					obj.部門名稱 = DisplayLookArr[i].split("_").pop();
					obj.姓名 = DisplayLookArr[i].split("_").shift();
					obj.帳號 = HideArr[i].split(",").shift();
					//obj.姓名 = HideArr[i].split("@").pop();
					//obj.部門名稱 = "100-資訊課";
					//obj.職稱 = "工程師";
					//obj.帳號 = "003121";
					//obj.姓名 = "劉俊成";
					objArr.push(obj);
					obj = {};
				}
				obj = {
					DisplayLookArr: $("[name=" + TmpName + "_name]").val().replace(/→/g, ","),
					JsonArr: objArr,
				}
			}
			//console.log(obj);

			openMultSelector({
				title: '請選擇對象',
				width: 1000,
				selectorID: 'SIRTEC_G_COMMUNICATION_InformMember',
				//ReplaceStr: $('[name=ApplicantDept]').val(),
				//ReplaceInStr: '',
				//url: 'SIRTEC_G_COMMUNICATION_GetCCMember.ASPX?DeptID=' + $('[name=receive_dept]').val(),
				//ApproverDept: $('[name=cc_dept]').val(),
				//ApproverID: $('[name=cc_id]').val(),
				UpDownBtn: 1,
				dataSource: 'Url',
				JsonArr: JSON.stringify(obj),
				func: function (selectorDialog, data) {
					var FuncAssistStr = "";
					var UnitCodeStr = "";

					var cc_dept = "";
					var cc_deptname = "";


					$.each(data, function (i, dataItem) {
						//console.log(data);
						if (FuncAssistStr == "") {
							FuncAssistStr += dataItem['部門名稱'] + "_" + dataItem['姓名'];
							UnitCodeStr += dataItem['帳號'];
						}
						else {
							FuncAssistStr += "," + dataItem['部門名稱'] + "_" + dataItem['姓名'];
							UnitCodeStr += "," + dataItem['帳號'];
						}

					});
					$("[name=" + TmpName + "_id]").val(UnitCodeStr);
					$("[name=" + TmpName + "_name]").val(FuncAssistStr);

					//$('[name=receive_dept]').val(cc_dept);
					//$('[name=receive_deptname]').val(cc_deptname);
					selectorDialog.dialog('close');
				}
			});
		});



		$('[name=Product_Quantity]').attr("type", "Number");
		$('[name=Required_Delivery_Date]').attr("type", "Date");

		$('[name=Pre-Budget]').on('change', function () {
			console.log('change');
		});

		//<隱藏採購單>
		$('#applicant-po').hide();
	});
}

/**簽核、檢視共同控制 */
function COMMENTLayout() {
	var ProcessID = $('[name=ProcessID]').val();
	//console.log(ProcessID);
	$().ready(function () {
		//表頭 - 單選按鈕處理
		var RadioObj = ["Priority"];
		$.each(RadioObj, function (index, value) {
			$('[name=' + value + ']:checked').removeAttr("disabled");
		});

		//表頭 - 
		var TextObj = ["COMPANYNAME", "company_code", "EXPENSE_CATEGORY_COMMENT"]
		$.each(TextObj, function (index, value) {
			hideEdit($('[name=' + value + ']'));
		});
		CompanyNameDisplay();

		//表頭表身-採購單


		if (ProcessID == 'AplDpt01' || ProcessID == 'RtAthCht01' || ProcessID == "DsntList01" || ProcessID == "RtSpcDpg01" || ProcessID == 'AplSlf01' || ProcessID == 'DsntList03' || ProcessID == 'AplSlf02') {
			$('#applicant-po').hide();
		} else {
			//alert('取得資料');
			$.ajax({
				url: "SIRTEC_G_MPR_GetPOContent.aspx",
				type: "POST",
				async: false,
				data: { "RequisitionID": bpm.formInfo.requisitionID, "ProcessID": bpm.formInfo.processID, "company_code": $('[name=company_code]').val() },
				dataType: "text",
				success: function (text) {
					$('#POContent').append(text);
					POContentCSS();
					if (ProcessID == 'DsntList02') {
						POContentJS();
					}
				},
				error: function (xhr, ajaxOptions, thrownError) {
					console.log(xhr);
				}
			});

			if (ProcessID == 'DsntList02') {
				$('div[name=Adraft]').removeAttr('onclick').on('click', function () {
					insertDetailData_POContent();
					ApproveSBTN('Adraft', '1', 'DraftFlag', '儲存草稿');
				});
			}
		}

		var pathname = location.pathname.replace('/Sirtec/', '').replace('/sirtec/', '').replace('.aspx', '');
		console.log(pathname);
		if (pathname == 'FM7_FormContent_Print_Auto_APPO') {
			$('#form').each(function () {
				$(this).find('#tab').find('ul').attr('margin', '0');
				$(this).find('#tab').find('li').each(function () {
					$(this).children().attr('font-size', '12px');
				});
			});

			$('#applicant').hide();
			$('#POContent,#header,#applicant-pr').each(function () {
				$(this)

				$(this).find('th,td').each(function () {
					$(this).css('font-size', '12px');
					$(this).css('padding', '0px');
					$(this).css('height', 'auto');
				});

				var id = $(this).attr('id');
				if (id == 'header') {
					//console.log('123');
					$(this).find('font').css('font-size', '18px');
					$(this).find('h1').css('font-size', '16px');
				}
			});
		}

	});
};

/**檢視畫面 */
function CONTENTLayout() {
	DisplayIdentityName();
	SIRTEC_G_FCT_GetCompanyInfo();
	GetSerialID();
	VIEWLayout('applicant');
	SIRTEC_G_FCT_GetDetailTable();
	ConvNumber('CREDIT_AMOUNT', 2);
	ConvNumber('forecast_amount', 2);
}

/**簽核畫面 */
function SIGNLayout() {
	DisplayIdentityName();
	SIRTEC_G_FCT_GetCompanyInfo();
	GetSerialID();
	VIEWLayout('applicant');
	SIRTEC_G_FCT_GetDetailTable();
	ConvNumber('CREDIT_AMOUNT', 2);
	ConvNumber('forecast_amount', 2);
}

/**表頭/表身 - input/select/button 開啟disabled功能 */
function disabledInput() {
	$('[name=Applicant] input,select,button').attr('disabled', "true");
}

/**表身 - 明細表修正項次編碼 */
function loadDetailNo() {
	$('[name=detailItem][detailtype=item]').each(function (i) {
		var o = $(this).find('[name=Num]');
		$(o).val(i + 1);
		hideEdit(o);
	});
}

/**表身 - 新增明細表事件 */
function loadDetailEvent() {
	//取消事件
	$('[name=detailItem][detailtype=item]').each(function (i) {
		//取消欄位事件(避免重複)
		$(this).find('[name=Pre_Budget]').unbind();


	});

	//加入事件
	//Product_Category_select 下拉式選單



	$('[name=Product_Category_select]').unbind();
	$('[name=Product_Category_select]').bind('change', function () {
		$(this).parent().find('input[name=Product_Category]').val($(this).val());
	});

	//PR_Dept_buttom
	$('[name=PR_Dept_button]').unbind();
	$('[name=PR_Dept_button]').bind('click', function () {
		var button = this;
		openSelector({
			selectorID: 'SIRTEC_G_MPR_GetCostCenter',
			FilterSelectorID: '',
			accountID: '(-:newtypefunc:Cookie("AccountID"):-)',
			Multiple: 0,//0:單選,1:多選 -
			dataSource: 'url',
			url: 'SIRTEC_G_MPR_GetCostCenter.ASPX?company_code=' + $('[name=company_code]').val(),
			title: '請選擇部門',
			height: 500,
			width: 500,
			ReplaceStr: '',
			ReplaceInStr: '',
			func: function (objDialog, data) {
				//console.log(data);
				$(button).parent().find('input[name=PR_DeptID]').val(data['ORG_ID']);
				$(button).parent().find('input[name=PR_DeptNAME]').val(data['DESCRIPTION']);
				objDialog.close();
			}
		});

	});

	$('[name=detailItem][detailtype=item]').each(function (i) {
		//加入欄位事件
		//明細表總和寫入預算餘額


		$(this).find('[name=Pre_Budget]').on('change', function () {
			var prebudget = 0;
			var Product_Quantity = 0;
			var Total = 0;
			$('[name=detailItem][detailtype=item]').each(function (i) {
				prebudget = parseInt($(this).find('[name=Pre_Budget]').val());
				Product_Quantity = parseInt($(this).find('[name=Product_Quantity]').val());
				//console.log(prebudget + ',' + typeof prebudget);
				//console.log(Product_Quantity + ',' + typeof Product_Quantity);
				if (isNaN(prebudget)) {
					prebudget = 0;
				}
				if (isNaN(Product_Quantity)) {
					Number = 0;
				}
				Total += prebudget * Product_Quantity;
			})
			//console.log(prebudget + ',' + typeof prebudget);
			$('[name=BUDGET_SURPLUS]').val(Total);
		});
	});
}

/**表身 - 新增明細表


 * 
 * @param Arr 明細表JSON
 */
function insertDetailDB(Arr) {
	$.ajax({
		url: "InsertDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_" + $('[name=Identify]').val() + "_D", "itemList": JSON.stringify(Arr) },
		dataType: "json",
		success: function (json) {
			console.log(json);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
}

/**表身 - 產生新增明細表JSON */
function insertDetailData() {
	var Arr = new Array;
	$('[name=detailItem][detailtype=item]').each(function (i) {
		var Obj = new Object;
		Obj.RequisitionID = bpm.formInfo.requisitionID;
		Obj.Num = $(this).find('[name=Num]').val();
		Obj.Product_Category = $(this).find('[name=Product_Category]').val();
		Obj.Product_Name = $(this).find('[name=Product_Name]').val();
		Obj.Product_Specification = $(this).find('[name=Product_Specification]').val();
		Obj.Product_Quantity = $(this).find('[name=Product_Quantity]').val();
		Obj.Required_Delivery_Date = $(this).find('[name=Required_Delivery_Date]').val();
		Obj.Instructions_For_Use = $(this).find('[name=Instructions_For_Use]').val();
		Obj.Pre_Budget = $(this).find('[name=Pre_Budget]').val();
		Obj.PR_DeptID = $(this).find('[name=PR_DeptID]').val();
		Obj.PR_DeptNAME = $(this).find('[name=PR_DeptNAME]').val();
		Arr.push(Obj);
	});
	console.log(Arr);
	deleteDetailDB();
	insertDetailDB(Arr);
}

/**表身 - 刪除明細表 */
function deleteDetailDB() {
	$.ajax({
		url: "DeleteDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_" + $('[name=Identify]').val() + "_D", "RequisitionID": bpm.formInfo.requisitionID },
		dataType: "json",
		success: function (json) {
			console.log(json);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
}

/**表身 - 顯示明細表


 * 
 * @param Arr
 */
function getDetailData(Arr) {
	$.each(Arr, function (i, json) {
		bpm.detail.addItem('detail');
		$('[name=detailItem][detailtype=item]:last').find('[name=Product_Category]').val(json.Product_Category);
		$('[name=detailItem][detailtype=item]:last').find('[name=Product_Name]').val(json.Product_Name);
		$('[name=detailItem][detailtype=item]:last').find('[name=Product_Specification]').val(json.Product_Specification);
		$('[name=detailItem][detailtype=item]:last').find('[name=Product_Quantity]').val(json.Product_Quantity);
		$('[name=detailItem][detailtype=item]:last').find('[name=Required_Delivery_Date]').val(json.Required_Delivery_Date.replace('1900-01-01', ''));
		$('[name=detailItem][detailtype=item]:last').find('[name=Instructions_For_Use]').val(json.Instructions_For_Use);
		$('[name=detailItem][detailtype=item]:last').find('[name=Pre_Budget]').val(json.Pre_Budget);
		//$('[name=detailItem][detailtype=item]:last').find('[name=PR_DeptID]').val(json.PR_DeptID);
		$('[name=detailItem][detailtype=item]:last').find('[name=PR_DeptNAME]').val(json.PR_DeptNAME);
	});
}

/**表身 - 檢視取得明細表JSON */
function getDetailDB() {
	$.ajax({
		url: "GetDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_" + $('[name=Identify]').val() + "_D", "RequisitionID": bpm.formInfo.requisitionID },
		dataType: "json",
		success: function (jsonArr) {
			//console.log(jsonArr);
			getDetailData(jsonArr);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
}

/**表身 - 檢視取得明細表JSON(重送表單/依此單內容重送表單) */
function getDetailDB_O() {
	$.ajax({
		url: "GetDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_" + $('[name=Identify]').val() + "_D", "RequisitionID": $('[name=ORequisitionID]').val() },
		dataType: "json",
		success: function (jsonArr) {
			console.log(jsonArr);
			getDetailData(jsonArr);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
}

/**表頭 -  更新資料
 * 
 * @param Arr 更新資料JSON
 */
function updateMDetailDB(Arr) {
	$.ajax({
		url: "UpdateDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_SIRTEC_G_MPR_M", "itemList": JSON.stringify(Arr), "RequisitionID": bpm.formInfo.requisitionID },
		dataType: "json",
		success: function (json) {
			console.log(json);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
}

/**表頭 -  產生更新資料JSON */
function updateMDetailData() {
	var ProcessID = $('[name=ProcessID]').val();
	var Arr = new Array;
	switch (ProcessID) {
		case 'DsntList02':
			var Obj = new Object;
			Obj.TOTAL_AMOUNT = $('[name=TOTAL_AMOUNT]').val();
			Arr.push(Obj);
			break;
		case 'AplSlf02':
			var Obj = new Object;
			Obj.EXPENSE_CATEGORY = $('[name=EXPENSE_CATEGORY]').val();
			Obj.BUDGET_SURPLUS = $('[name=BUDGET_SURPLUS]').val();
			Obj.PO_MEMBERID = $('[name=PO_MEMBERID]').val();
			Obj.PO_MEMBERNAME = $('[name=PO_MEMBERNAME]').val();
			Obj.TOTAL_AMOUNT = $('[name=TOTAL_AMOUNT]').val();
			Obj.MutiSign_id = $('[name=MutiSign_id]').val();
			Obj.MutiSign_name = $('[name=MutiSign_name]').val();
			Arr.push(Obj);
			break;
		default:
			break;
	}
	console.log(Arr);
	updateMDetailDB(Arr);
}

/**表頭 - 初始設定 */
function init() {
	//<明細表前置作業>
	getDetailDB();
	loadDetailEvent();
	var RequisitionID = $('[name=RequisitionID]').val();
	var ORequisitionID = $('[name=ORequisitionID]').val();
	if (ORequisitionID == null || ORequisitionID == '') {
		return;
	} else {
		if (RequisitionID !== ORequisitionID) {
			getDetailDB_O();
		}
	}
}

/**表頭 - 選擇公司別(下拉式選單) */
function SearchCompanyName() {
	var ApplicantDept = $('[name=ApplicantDept]').val();
	var ApplicantDeptArray = ApplicantDept.split("-");
	var company_code = ApplicantDeptArray[0];
	$.ajax({
		type: "POST",
		url: "SearchCompany.aspx",
		data: { company_code: company_code },
		async: false,
		dataType: "xml",
		success: function (msg) {
			xml = msg;
		},
		error: function (e) {
			console.log("Error-SearchCompanyName(SearchCompany.aspx)");
		}
	});
	$('[name=COMPANY_PREFIX_CODE] option').remove();
	$(xml).find("xml").each(function () {
		$(this).children("AutoCounter").each(function () {
			var COMPANY_SHORT_NAME = $(this).children("COMPANY_SHORT_NAME").text();
			$('[name=COMPANY_PREFIX_CODE]').append($("<option></option>").attr("value", COMPANY_SHORT_NAME).text(COMPANY_SHORT_NAME));
		});
	});
}

/**表頭 - 顯示公司名稱 */
function CompanyNameDisplay() {
	var COMPANY_SHORT_NAME = $('[name=COMPANY_PREFIX_CODE]').val();
	$.ajax({
		type: "POST",
		url: "CompanyName.aspx",
		data: { COMPANY_SHORT_NAME: COMPANY_SHORT_NAME },
		async: false,
		dataType: "xml",
		success: function (msg) {
			xml = msg;
		},
		error: function (e) {
			console.log("Error-CompanyNameDisplay(CompanyName.aspx)");
		}
	});
	var company_code = "";
	$(xml).find("xml").children("AutoCounter").each(function () {
		var CompanyFullName = $(this).children("CompanyFullName").text();
		company_code = $(this).children("prefix_code").text();
		$('#COMPANYNAME').text(CompanyFullName);
		$('[name=COMPANYNAME]').val(CompanyFullName);
		$('[name=company_code]').val(company_code);
		hideEdit($('[name=COMPANYNAME]'))
		hideEdit($('[name=company_code]'))
	});
}

/**請購單 - 顯示採購人員(下拉式選單) */
function ChangeSelectItem() {
	$.ajax({
		type: "POST",
		url: "SIRTEC_G_MPR_GetSelect_POMember_Option.aspx",
		data: {
			Company_code: $('[name=company_code]').val(),
			ApplicantID: $('[name=ApplicantID]').val(),
			ApplicantDept: $('[name=ApplicantDept]').val(),
		},
		async: false,
		dataType: "text",
		success: function (text) {
			//console.log(text);
			$('[name=POMember_SelectItem]').empty();
			$('[name=POMember_SelectItem]').append(text);
		}, error: function (e) {
			console.log("Error-DisplayIdentityName");
		}
	});
}

/**請購單 - 欄位轉換金額
 * 
 * @param ColumnName 欄位名稱
 */
function HideAmountCloumn(ColumnName) {
	//console.log(ColumnName);
	var transCompany_code_FC_INT = parseInt($('[name=company_code]').val().substring(0, 1));//company_code,string
	//console.log(transCompany_code_FC_INT);
	var VALUES_STRING = ($('[name=' + ColumnName + ']').val() != '') ? $('[name=' + ColumnName + ']').val() : '0.00';
	//console.log(VALUES_STRING);
	var VALUES_SURPLUS_FLOAT = parseInt(VALUES_STRING);
	//console.log(VALUES_SURPLUS_FLOAT);
	var VALUES_SURPLUS_DECIMAL = (transCompany_code_FC_INT == 1) ? 0 : 2;
	//console.log(VALUES_SURPLUS_DECIMAL);
	var VALUES_SURPLUS_MARK = (transCompany_code_FC_INT == 1) ? '$' : '¥';
	//console.log(VALUES_SURPLUS_MARK);
	var transVALUES = VALUES_SURPLUS_MARK + VALUES_SURPLUS_FLOAT.toFixed(VALUES_SURPLUS_DECIMAL).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
	//console.log(transVALUES);
	$('[name=' + ColumnName + ']').parent().find('[name=transVALUES]').empty();
	$('[name=' + ColumnName + ']').parent().append('<font name="transVALUES">' + transVALUES + '</font>');
	$('[name=' + ColumnName + ']').hide();
};

/**請購單 - 明細表轉換金額


 * 
 * @param ColumnName 欄位名稱
 */
function HideAmountCloumn_Detail(ColumnName) {
	var transCompany_code_FC_INT = parseInt($('[name=company_code]').val().substring(0, 1));//company_code,string
	var VALUES_STRING = (ColumnName.val() != '') ? ColumnName.val() : '0.00';
	var VALUES_SURPLUS_FLOAT = parseInt(VALUES_STRING);
	var VALUES_SURPLUS_DECIMAL = (transCompany_code_FC_INT == 1) ? 0 : 2;
	var VALUES_SURPLUS_MARK = (transCompany_code_FC_INT == 1) ? '$' : '¥';
	var transVALUES = VALUES_SURPLUS_MARK + VALUES_SURPLUS_FLOAT.toFixed(VALUES_SURPLUS_DECIMAL).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
	ColumnName.parent().append(transVALUES);
	ColumnName.hide();
};

/**採購明細表 - CSS */
function POContentCSS() {
	var ProcessID = $('[name=ProcessID]').val();
	if (ProcessID == 'DsntList02') {
		$('[name=POContent_table]').each(function (i) {
			console.log(i);
			$(this).css("table-layout", "fixed");
			$(this).find('thead tr th').each(function (i) {
				if (i < 2) {
					$(this).css("width", "20px");
				}

				if (i > 1 && i < 5) {

					$(this).hide();
				}

				if (i == 6) {

					$(this).hide();
				}

				if (i >= 5) {
					$(this).css("width", "100px");
				}

				if (i > 7) {
					$(this).css("width", "50px");
				}
			});

			$(this).find('tbody tr').each(function () {
				$(this).find('td').each(function (i) {
					if (i < 2) {
						$(this).css("text-align", "center")
					}

					if (i > 1 && i < 5 || i == 6) {
						$(this).hide();
					}


					if (i >= 8 && i <= 12) {
						$(this).find('input').css("text-align", "right");
						$(this).css("text-align", "right");
					}

					if (i == 6) {
						$(this).hide();
					}

					if (i == 8) {
						$(this).find('input[type=text]').attr("readonly", "readonly");
					}


					if (i == 10) {
						$(this).find('input[type=text]').attr("readonly", "readonly");
					}

					if (i == 12) {
						$(this).find('input[type=text]').attr("readonly", "readonly");
					}

					$('input').each(function () {
						var name = $(this).attr('name');
						if (name == 'ItemCount' || name == 'ItemAmount' || name == 'TotalTax' || name == 'TotalAmount') {
							$('[name=' + name + ']').attr("readonly", "readonly");
						}
					});

				});
			});
		});
	} else {
		//彙整資料
		$('#POContent').find('table').each(function (i) {
			if (i == 1) {
				$(this).find('thead tr th').each(function (i) {
					if (i == 1 || i == 2 || i == 3 || i == 5) {
						$(this).hide();
					}
				});
				$(this).find('tbody tr').each(function (i) {
					$(this).find('td').each(function (i) {
						if (i == 1 || i == 2 || i == 3 || i == 5) {
							$(this).hide();
						}
					});
				});
			}
		});

		//比價資料
		$('[name=POContent_table]').each(function (i) {
			//表格明細表分配寬度


			$(this).css("table-layout", "fixed");

			//表格明細表標題(thead)
			$(this).find('thead tr th').each(function (i) {
				//調整表格明細表(th)寬距
				if (i < 2) {
					$(this).css("width", "20px");
				}

				//隱藏表格明細表(th)
				if ((i > 1 && i < 5) || (i == 6)) {
					$(this).hide();
				}

			});

			//表格明細表標題(tbody)
			$(this).find('tbody tr').each(function (i) {
				$(this).find('td').each(function (i) {
					//調整表格明細表(td)寬距
					if (i < 2) {
						$(this).css("text-align", "center")
					}

					//調整表格明細表(td)寬距-數字靠右
					if (i > 7) {
						$(this).css("text-align", "right")
					}

					//隱藏表格明細表(td)
					if ((i > 1 && i < 5) || (i == 6)) {
						$(this).hide();
					}

				});
			});
		});
	}

}

/**採購明細表 - 事件 */
function POContentJS() {
	var company_code = $('[name=company_code]').val();
	if (company_code == '100') {
		$('[name=TaxRate]').val(5);
		$('[name=TaxRate]').attr('readonly', 'readonly');
	}

	$('[name=ItemPrice],[name=TaxRate],[name=TotalTax],[name=TotalAmount]').on('click change', function () {
		var tr = $(this).closest('tr');
		var ItemCount = tr.find('[name=ItemCount]').val() ?? '0';
		var ItemPrice = tr.find('[name=ItemPrice]').val() ?? '0';
		var ItemAmount = parseFloat(ItemCount) * parseFloat(ItemPrice);
		tr.find('[name=ItemAmount]').val(ItemAmount);

		var TaxRate = tr.find('[name=TaxRate]').val() ?? '0';
		var TotalTax = ItemAmount * parseFloat(TaxRate) / 100;
		tr.find('[name=TotalTax]').val(TotalTax);

		var TotalAmount = ItemAmount + TotalTax;
		tr.find('[name=TotalAmount]').val(TotalAmount);
		GetTOTAL_AMOUNT();
	});
}

/**採購單明細表 - 產生JSON*/
function insertDetailData_POContent() {
	var Arr = new Array;
	$('[name=POContent_table]').each(function (i) {
		console.log(i);
		$(this).find('tbody tr').each(function () {

			var SelectFlag = $(this).find('td:first input[type=radio]:checked').val();
			if (SelectFlag == 'on') {
				SelectFlag = 'Y';
			} else {
				SelectFlag = 'N';
			}

			var Obj = new Object;
			Obj.SelectFlag = SelectFlag;
			Obj.Num = $(this).find('td input[name=Num]').val();
			Obj.RequisitionID = $(this).find('td input[name=RequisitionID]').val();
			Obj.Product_Name = $(this).find('td input[name=Product_Name]').val();
			Obj.Product_Specification = $(this).find('td input[name=Product_Specification]').val();
			Obj.Customer = $(this).find('td input[name=Customer]').val();
			Obj.ItemName = $(this).find('td input[name=ItemName]').val();
			Obj.ItemSpec = $(this).find('td input[name=ItemSpec]').val();
			Obj.ItemCount = $(this).find('td input[name=ItemCount]').val();
			Obj.ItemPrice = $(this).find('td input[name=ItemPrice]').val();
			Obj.ItemAmount = $(this).find('td input[name=ItemAmount]').val();
			Obj.TaxRate = $(this).find('td input[name=TaxRate]').val();
			Obj.TotalTax = $(this).find('td input[name=TotalTax]').val();
			Obj.TotalAmount = $(this).find('td input[name=TotalAmount]').val();
			Obj.comment = $(this).find('td input[name=comment]').val();
			Arr.push(Obj);

		})
	});
	console.log(Arr);
	deleteDetailDB_POContent();
	insertDetailDB_POContent(Arr);
}

/**採購單明細表 -  刪除明細表 */
function deleteDetailDB_POContent() {
	$.ajax({
		url: "DeleteDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_SIRTEC_G_MPR_D_POContent", "RequisitionID": bpm.formInfo.requisitionID },
		dataType: "json",
		success: function (json) {
			console.log(json);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
}

/** 採購單明細表 -  新增明細表


 * 
 * @param Arr 明細表JSON
 */
function insertDetailDB_POContent(Arr) {
	$.ajax({
		url: "InsertDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_SIRTEC_G_MPR_D_POContent", "itemList": JSON.stringify(Arr) },
		dataType: "json",
		success: function (json) {
			console.log(json);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
}

/**採購單明細表 - 總費用 */
function GetTOTAL_AMOUNT() {
	var TOTAL_AMOUNT = parseFloat($('[name=TOTAL_AMOUNT]').val());
	//console.log(TOTAL_AMOUNT);
	var SUB_TOTAL_AMOUNT = 0;
	$('[name=POContent_table]').each(function (i) {
		$(this).find('tbody tr').each(function (i) {
			var check = $(this).find('td:first input:checked').val();
			if (check == 'on') {
				SUB_TOTAL_AMOUNT += parseFloat($(this).find('td input[name=TotalAmount]').val());
			} else {
				return;
			}
		})
	});
	TOTAL_AMOUNT = SUB_TOTAL_AMOUNT;
	$('[name=TOTAL_AMOUNT]').val(TOTAL_AMOUNT);
	HideAmountCloumn('TOTAL_AMOUNT');
};

/**採購單明細表 - 總費用 */
function HideEditPOContent() {
	$('[name=POContent_tr]').each(function () {
		hideEdit($(this).find('td input[name=PO_Customer]'));
		hideEdit($(this).find('td input[name=PO_ItemName]'));
		hideEdit($(this).find('td input[name=PO_ItemSpec]'));
		hideEdit($(this).find('td input[name=PO_ItemCount]'));
		hideEdit($(this).find('td input[name=PO_ItemPrice]'));
		hideEdit($(this).find('td input[name=PO_ItemAmount]'));
	});
}

/**採購單明細表 - 產生新增明細表JSON(重送表單/依此單內容重送表單) */
function insertDetailData_POContent_O() {
	var Arr = new Array;
	$.ajax({
		url: "GetDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_SIRTEC_G_MPR_D_POContent", "RequisitionID": $('[name=ORequisitionID]').val() },
		dataType: "json",
		success: function (jsonArr) {
			//console.log(jsonArr);
			$.each(jsonArr, function (i, json) {
				var Obj = new Object;
				Obj.RequisitionID = bpm.formInfo.requisitionID;
				Obj.SelectFlag = json.SelectFlag;
				Obj.Num = json.Num;
				Obj.Product_Name = json.Product_Name;
				Obj.Product_Specification = json.Product_Specification;
				Obj.Customer = json.Customer;
				Obj.ItemName = json.ItemName;
				Obj.ItemSpec = json.ItemSpec;
				Obj.ItemCount = json.ItemCount;
				Obj.ItemPrice = json.ItemPrice;
				Obj.TaxRate = json.TaxRate;
				Obj.TotalTax = json.TotalTax;
				Obj.TotalAmount = json.TotalAmount;
				Obj.comment = json.Comment;
				Arr.push(Obj);
			});
			//deleteDetailDB_POContent_O();

		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
	//console.log('New,' + bpm.formInfo.requisitionID);
	//console.log('Old,' + $('[name=ORequisitionID]').val());
	console.log(Arr);
	deleteDetailDB_POContent_O();
	insertDetailDB_POContent_O(Arr);
};

/**採購單明細表 -  刪除明細表(重送表單/依此單內容重送表單) */
function deleteDetailDB_POContent_O() {
	$.ajax({
		url: "DeleteDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_SIRTEC_G_MPR_D_POContent", "RequisitionID": bpm.formInfo.requisitionID },
		dataType: "json",
		success: function (json) {
			console.log(json);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
}

/** 採購單明細表 -  新增明細表(重送表單/依此單內容重送表單)
 * 
 * @param Arr 明細表JSON
 */
function insertDetailDB_POContent_O(Arr) {
	console.log(Arr);
	$.ajax({
		url: "InsertDetailDB.aspx",
		type: "POST",
		async: false,
		data: { "table": "FM7T_SIRTEC_G_MPR_D_POContent", "itemList": JSON.stringify(Arr) },
		dataType: "json",
		success: function (json) {
			console.log(json);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Ajax發生錯誤");
			console.log(xhr);
		}
	});
}

