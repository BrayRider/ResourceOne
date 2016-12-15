function setupRoleGrid(gridName, prefix, urlIn, dblClick, lcFn) {
    jQuery(gridName).jqGrid({
        url: urlIn,
        datatype: 'json',
        mtype: 'POST',
        height: 160,
        ondblClickRow: dblClick,
        altRows: true,
        altclass: 'ui-widget-contentalt',
        colNames: ['ID', 'Role Name', 'Description', 'Exception'],

        colModel: [
                          { name: 'ID', index: 'ID', width: 0, align: 'left', hidden: true, search: false },
                          { name: 'Name', index: 'Name', width: 160, align: 'left',  searchoptions: { sopt: ['cn']} },
                          { name: 'Description', index: 'Description', width: 240, align: 'left',  searchoptions: { sopt: ['cn']} },
                          { name: 'Exception', index: 'Exception', width: 0, align: 'left', hidden: true, search: false },
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
        caption: prefix + ' Roles'
    }).navGrid('#pager', { search: true, edit: false, add: false, del: false, searchtext: "Search" });
}


function setupRoleGridWithExceptions(gridName, prefix, urlIn, dblClick, lcFn) {
    jQuery(gridName).jqGrid({
        url: urlIn,
        datatype: 'json',
        mtype: 'POST',
        height: 160,
        ondblClickRow: dblClick,
        altRows: true,
        altclass: 'ui-widget-contentalt',
        colNames: ['ID', 'Role Name', 'Description', 'Exception'],

        colModel: [
                          { name: 'ID', index: 'ID', width: 0, align: 'left', hidden: true, search: false },
                          { name: 'Name', index: 'Name', width: 160, align: 'left', formatter: formatExeption, unformat: unformatCell, searchoptions: { sopt: ['cn']} },
                          { name: 'Description', index: 'Description', width: 240, align: 'left', formatter: formatExeption, unformat: unformatCell, searchoptions: { sopt: ['cn']} },
                          { name: 'Exception', index: 'Exception', width: 1, align: 'left', hidden: true, search: false },
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
        caption: prefix + ' Roles'
    }).navGrid('#pager', { search: true, edit: false, add: false, del: false, searchtext: "Search" });
}

function formatExeption(cellValue, mask, rowObject) {
    var isExcept = parseInt(rowObject[3]);
    if (isExcept == 0) {
        return "<span class='autoAdded'>" + cellValue + "</span>";
    }
    

    return  "<span class='userAdded'>" + cellValue + "</span>";;
}

function unformatCell(cellvalue, options, cellobject) {

    return cellvalue;
}