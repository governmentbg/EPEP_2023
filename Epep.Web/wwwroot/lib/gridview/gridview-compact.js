var GridViewInstances = [];

class GridView {
    constructor(options) {
        this.state = {};
        this.state.container = options.container;
        $(this.state.container).addClass('gridview-container');
        if (options.class) {
            $(this.state.container).addClass(options.class);
        }
        this.state.url = options.url;
        this.state.size = options.size ?? 10;
        this.state.xlsExport = options.xlsExport ?? false;
        this.state.pager = options.pager ?? true;
        this.state.sizeSelector = options.sizeSelector ?? true;
        this.state.method = options.method ?? 'POST';
        this.state.data = options.data;
        this.state.template = options.template;
        this.state.full_template = options.full_template;
        this.state.view_top_pages = options.view_top_pages ?? true;
        this.state.view_all_url = options.view_all_url;
        this.state.loader_text = options.loader_text || '';
        this.state.empty_text = options.empty_text || 'Няма намерени елементи.';
        this.state.grid_title = options.grid_title || '';
        this.state.grid_title_class = options.grid_title_class || '';
        this.state.loader = options.loader;
        GridViewInstances.push(this);
        if (options.autoload != false) {
            $(this.state.container).html(this.state.loader_text);
            this.loadData(1);
        }
    }

    loadData(pageNo) {
        this.state.page = pageNo;
        var gridRequest = new Object();
        if (this.state.data)
            gridRequest.data = JSON.stringify(this.state.data());
        gridRequest.page = this.state.page;
        gridRequest.size = this.state.size;
        fetch(this.state.url,
            {
                method: this.state.method,
                body: JSON.stringify(gridRequest),
                headers: {
                    'Cache-Control': 'no-cache',
                    'Content-Type': 'application/json'
                }
            }
        )
            .then((response) => response.json())
            .then((data) => {
                this.showData(data);
            }
            ).catch((error) => {
                console.log(error);
            });
    }

    exportData(exportFormat) {
        var gridRequest = new Object();
        if (this.state.data)
            gridRequest.data = JSON.stringify(this.state.data());
        gridRequest.exportFormat = exportFormat;
        let fileName = 'report.xlsx';
        fetch(this.state.url,
            {
                method: this.state.method,
                body: JSON.stringify(gridRequest),
                headers: {
                    'Cache-Control': 'no-cache',
                    'Content-Type': 'application/json'
                }
            }
        )
            .then(res => res.blob())
            .then((blob) => {
                //var file = window.URL.createObjectURL(blob);
                //window.location.assign(file);

                var url = window.URL.createObjectURL(blob);
                var a = document.createElement('a');
                a.href = url;
                a.download = fileName;
                document.body.appendChild(a); // append the element to the dom
                a.click();
                a.remove(); // afterwards, remove the element  
            })
            .catch((error) => {
                console.log(error);
            });
    }

    showData(result) {
        if (!$(this.state.container).data('loaded')) {
            $(this.state.container).data('loaded', 'loaded');
        }
        if (result.data && result.data.length == 0 && this.state.empty_text) {
            $(this.state.container).html(this.state.empty_text);
            return;
        }
        var pages = [];
        if (result.total_pages > 1) {
            for (var i = 1; i <= result.total_pages; i++) {
                pages.push(i);
            }
        }
        result.container = this.state.container;

        var tmpl = '';
        if (this.state.template) {
            tmpl += this.templateStartEpep(result);
            tmpl += $(this.state.template).html();
            tmpl += this.templateEndEpep(result);
        }
        if (this.state.full_template) {
            tmpl = $(this.state.template).html();
        }
        var hbars = Handlebars.compile(tmpl);
        try {
            hbars();
        } catch (error) {
            var encodedError = error.message.replace(/[\u00A0-\u9999<>\&]/gim, function (i) {
                return '&#' + i.charCodeAt(0) + ';';
            });
            console.log('<pre>' + encodedError + '</pre>');
        }
        $(this.state.container).html(hbars(
            {
                data: result.data,
                notFirstPage: result.page > 1,
                notLastPage: result.page < result.total_pages,
                page: result.page,
                container: this.state.container,
                pages: pages
            }));
    }
    templateCurrentRows(data) {
        if (!this.state.view_top_pages && !this.state.view_all_url) {
            return '';
        }
        let from = data.size * (data.page - 1) + 1;
        let to = from + data.size - 1;
        if (to > data.total_rows) {
            to = data.total_rows
        }
        let result =
            '<div class="col-auto">' +
            '<div class="pagination">' +
            '<nav aria-label="Странициране на документи">';
        if (this.state.view_top_pages) {
            result += `<li class="page-location">${from}-${to} от ${data.total_rows}</li>`;
        }
        result += '</ul>' +
            '</nav></div></div>';
        if (this.state.view_all_url) {
            result += `<div class="col-auto"><a href="${this.state.view_all_url}" class="">Виж всички</a></div>`;
        }
        return result;
    }
    setSize(newSize) {
        this.state.size = newSize;
        this.loadData(1);
    }

    templateSizeSelector(selectedSize) {
        var sizes = [];
        //sizes.push(5);
        sizes.push(10);
        sizes.push(20);
        sizes.push(50);
        //sizes.push(1000000);

        return this.templateSizeSelectorEpep(selectedSize, sizes);
    }
    //templateSizeSelectorCombo(selectedSize, sizes) {
    //    var result = '<div class="gridView-size-selector pull-right">Покажи: ';
    //    result += '<select onchange="gridViewSetSize(\'' + this.state.container + '\',$(this).val());return false;">'
    //    for (var i = 0; i < sizes.length; i++) {
    //        var selected = '';
    //        if (selectedSize == sizes[i]) {
    //            selected = ' selected="selected"';
    //        }
    //        var allText = sizes[i].toString();
    //        if (sizes[i] > 1000) {
    //            allText = "Всички";
    //        }
    //        result += '<option ' + selected + ' value="' + sizes[i] + '">' + allText + '</option>';
    //    }
    //    result += '</select></div>';
    //    return result;
    //}
    //templateSizeSelectorLinks(selectedSize, sizes) {
    //    var result = '<div class="gridView-size-selector pull-right">Покажи: ';
    //    for (var i = 0; i < sizes.length; i++) {
    //        var selected = '';
    //        if (selectedSize == sizes[i]) {
    //            selected = ' class="selected"';
    //        }
    //        var allText = sizes[i].toString();
    //        if (sizes[i] > 1000) {
    //            allText = "Всички";
    //        }
    //        result += '<a href="#" onclick="gridViewSetSize(\'' + this.state.container + '\',' + sizes[i] + ');return false;"' + selected + '>' + allText + '</a>';
    //    }
    //    result += '</div>';
    //    return result;
    //}
    //templateStart() {
    //    return '{{#each this.data}}';
    //}
    templateExportXls() {
        var result = '<form action="" class="u-form col-auto ms-auto row">';
        result += '<div class="u-form-cell col-auto"><label onclick="gridViewExportData(\'' + this.state.container + '\',\'xls\');return false;">Експорт в Ескел</label>';
        result += '</div></form>';
        return result;
    }
    templateSizeSelectorEpep(selectedSize, sizes) {
        var result = '<form action="" class="u-form col-auto ms-auto row">';
        result += '<div class="u-form-cell col-auto"><label>Покажи по </label>';
        result += '<select class="form-select" aria-label="Филтрирай по" onchange="gridViewSetSize(\'' + this.state.container + '\',$(this).val());return false;">'
        for (var i = 0; i < sizes.length; i++) {
            var selected = '';
            if (selectedSize == sizes[i]) {
                selected = ' selected="selected"';
            }
            var allText = sizes[i].toString();
            if (sizes[i] > 1000) {
                allText = "Всички";
            }
            result += '<option ' + selected + ' value="' + sizes[i] + '">' + allText + '</option>';
        }
        result += '</select><small>на страница</small></div></form>';
        return result;
    }
    templateStartEpep(data) {
        let startTemplate = '<section class="section">' +
            '<div class="section-header" >' +
            '<div class="row">' +
            `<div class="col-auto"><h2 class="section-title ${this.state.grid_title_class}">${this.state.grid_title}</h2></div>`;
        if (this.state.sizeSelector === true) {
            startTemplate += this.templateSizeSelector(data.size);
        }
        if (this.state.xlsExport === true) {
            startTemplate += this.templateExportXls();
        }
        startTemplate += this.templateCurrentRows(data);

        startTemplate += '</div>' +
            '</div>' +
            '<div class="section-body">' +
            '<ul class="list">' +
            '{{#each this.data}}';

        return startTemplate;
    }
    generatePageNumbers(page, totalPages) {
        let from = page - 5;
        if (from < 1) {
            from = 1;
        }
        let to = page + 5;
        if (to > totalPages) {
            to = totalPages;
        }
        let pageNumbers = [];
        for (var pageNo = from; pageNo <= to; pageNo++) {
            pageNumbers.push(pageNo);
        }
        return pageNumbers;
    }
    //templateEndButtons() {
    //    return '{{/each}}{{#if this.pages}}<div class="gridview-footer">{{#each this.pages}}<a href="#" onclick="gridViewLoadData(\'{{../container}}\',{{this}});return false;" class="{{#compare this ../page operator="=="}}selected{{/compare}}">{{this}}</a>{{/each}}</div>{{/if}}';
    //}
    templateEndEpep(data) {
        let templateEnd = '{{/each}}</ul></div></section >';
        if (this.state.pager === true) {
            templateEnd += this.templatePagerEpep(data);
        }
        return templateEnd;
    }
    //templatePager() {
    //    return '{{#if this.pages}}<div class="gridview-footer">' +
    //        '{{#if this.notFirstPage}}<a href="#" onclick="gridViewLoadData(\'{{this.container}}\',{{this.page}}-1);return false;">&lt;</a>{{/if}}' +
    //        '&nbsp;<b>{{this.page}}</b>&nbsp;' +
    //        '{{#if this.notLastPage}}<a href="#" onclick="gridViewLoadData(\'{{this.container}}\',{{this.page}}+1);return false;">&gt;</a>{{/if}}' +
    //        '</div>{{/if}}';
    //}
    templatePagerEpep(data) {
        let result = '<div class="page-pagination">' +
            '<nav aria-label="Странициране на документи">' +
            '<ul class="pagination">';
        if (data.page > 1) {
            result += '<li class="page-item page-first">';
            result += `<a class="page-link" title="Първа" href="#" onclick="gridViewLoadData('${data.container}',${1});return false;"><span class="visually-hidden">first</span></a>`;
            result += '</li>';
        } else {
            result += '<li class="page-item page-first page-inactive">';
            result += `<a class="page-link" title="Първа"><span class="visually-hidden">first</span></a>`;
            result += '</li>';
        }

        if (data.page > 1) {
            result += '<li class="page-item page-prev">';
            result += `<a class="page-link" title="Предна" href="#" onclick="gridViewLoadData('${data.container}',${data.page - 1});return false;"><span class="visually-hidden">first</span></a>`;
            result += '</li>';
        } else {
            result += '<li class="page-item page-prev page-inactive">';
            result += `<a class="page-link" title="Предна"><span class="visually-hidden">first</span></a>`;
            result += '</li>';
        }

        let pageNumbers = this.generatePageNumbers(data.page, data.total_pages);
        for (var i = 0; i < pageNumbers.length; i++) {
            if (pageNumbers[i] == data.page) {
                result += '<li class="page-item active">';
                result += `<a class="page-link" href="#" onclick="return false;" >${pageNumbers[i]}</a>`;
            } else {
                result += '<li class="page-item">';
                result += `<a class="page-link" href="#" onclick="gridViewLoadData('${data.container}',${pageNumbers[i]});return false;">${pageNumbers[i]}</a>`;
            }
            result += '</li>';
        }

        if (data.page < data.total_pages) {
            result += '<li class="page-item page-next">';
            result += `<a class="page-link" title="Следваща" href="#" onclick="gridViewLoadData('${data.container}',${data.page + 1});return false;"><span class="visually-hidden">next</span></a>`;
            result += '</li>';
        } else {
            result += '<li class="page-item page-next page-inactive">';
            result += `<a class="page-link" title="Следваща"><span class="visually-hidden">next</span></a>`;
            result += '</li>';
        }
        if (data.page < data.total_pages) {
            result += '<li class="page-item page-last">';
            result += `<a class="page-link" title="Последна" href="#" onclick="gridViewLoadData('${data.container}',${data.total_pages});return false;"><span class="visually-hidden">last</span></a>`;
            result += '</li>';
        } else {
            result += '<li class="page-item page-last page-inactive">';
            result += `<a class="page-link" title="Последна" ><span class="visually-hidden">last</span></a>`;
            result += '</li>';
        }

        result += '</ul></nav></div>';
        return result;
    }
    templateEndSimple() {
        return '{{/each}}{{#if this.pages}}<div class="gridview-footer">' +
            '{{#if this.notFirstPage}}<a href="#" onclick="gridViewLoadData(\'{{this.container}}\',{{this.page}}-1);return false;">&lt;</a>{{/if}}' +
            '&nbsp;<b>{{this.page}}</b>&nbsp;' +
            '{{#if this.notLastPage}}<a href="#" onclick="gridViewLoadData(\'{{this.container}}\',{{this.page}}+1);return false;">&gt;</a>{{/if}}' +
            '</div>{{/if}}';
    }
}

function gridViewFindByContainer(container) {
    for (var i = 0; i < GridViewInstances.length; i++) {
        if (GridViewInstances[i].state.container == container) {
            return GridViewInstances[i];
            break;
        }
    }
}

function gridViewSetSize(container, size) {
    let gridView = gridViewFindByContainer(container);
    gridView.setSize(size);
}

function gridViewExportData(container, exportFormat) {

    try {
        let gridView = gridViewFindByContainer(container);
        gridView.exportData(exportFormat);
    } catch (e) {
        console.log(e);
    }
}

function gridViewLoadData(container, page) {

    try {
        let gridView = gridViewFindByContainer(container);
        gridView.loadData(page || 1);
    } catch (e) {
        console.log(e);
    }
}

function gridViewDifferLoad(container) {
    if ($(container).data('loaded')) {
        return;
    }
    let gridView = gridViewFindByContainer(container);
    gridView.loadData(1);
}
