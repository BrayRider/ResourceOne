function setupLevelGrid(gridName, prefix, urlIn, dblClick, lcFn) {
    jQuery(gridName).jqGrid({
        url: urlIn,
        datatype: 'json',
        mtype: 'POST',
        height: 200,
        ondblClickRow: dblClick,
        altRows: true,
        altclass: 'ui-widget-contentalt',
        colNames: ['ID', 'Level Name', 'Description'],

        colModel: [
                          { name: 'ID', index: 'ID', width: 0, align: 'left', hidden: true, search: false },
                          { name: 'Name', index: 'Name', width: 150, align: 'left', searchoptions: { sopt: ['cn']} },
                          { name: 'Description', index: 'Description', width: 175, align: 'left', searchoptions: { sopt: ['cn']} }
                          ],
        //pager: jQuery('#pager'),
        rowNum: -1,
        sortname: 'Name',
        sortorder: "asc",
        viewrecords: true,
        loadonce: true,
        sortable: true,
        loadComplete: lcFn,
        imgpath: '../../Content/images',
        caption: prefix + ' Levels'
    }).navGrid('#pager', { search: true, edit: false, add: false, del: false, searchtext: "Search" });
}



function AssignLevel() {
    CopySelected("#alist", "#blist");

}

function assignByDoubleClick(rowid, iRow, iCol, e) {
    var row = jQuery(this).jqGrid('getRowData', rowid);
    jQuery(this).delRowData(rowid);

    jQuery("#blist").addRowData("#alist" + rowid, row);
    storeAssignedInForm();
}

function removeByDoubleClick(rowid, iRow, iCol, e) {
    var row = jQuery(this).jqGrid('getRowData', rowid);
    jQuery(this).delRowData(rowid);

    jQuery("#alist").addRowData("#blist" + rowid, row);
    storeAssignedInForm();
}

function noOpDoubleClick(rowid, iRow, iCol, e) {
 }

function storeAssignedInForm() {
    var assingedGrid = jQuery("#blist");
    var rows = assingedGrid.jqGrid('getRowData');
    $('#gridData').val(JSON.stringify(rows));
    //alert($('#gridData').val());
}

function CopySelected(fromGrid, toGrid) {
    var grid = jQuery(fromGrid);
    var rowKey = grid.getGridParam("selrow");
    if (rowKey != null) {
        toGrid = jQuery(toGrid);
        var row = grid.jqGrid('getRowData', rowKey);
        grid.delRowData(rowKey);

        toGrid.addRowData(fromGrid + rowKey, row);


        storeAssignedInForm();
    }
}


function RemoveLevel() {
    CopySelected("#blist", "#alist");
}

function assignByDoubleClickWithException(rowid, iRow, iCol, e) {
    var row = jQuery(this).jqGrid('getRowData', rowid);
    jQuery(this).delRowData(rowid);

    row[3] = "1";
    
    jQuery("#blist").addRowData("#alist" + rowid, row);
    storeAssignedInForm();
}