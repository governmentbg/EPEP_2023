Handlebars.registerHelper('compare', function (lvalue, rvalue, options) {

    if (arguments.length < 3)
        throw new Error("Handlerbars Helper 'compare' needs 2 parameters");

    operator = options.hash.operator || "==";

    var operators = {
        '==': function (l, r) { return l == r; },
        '===': function (l, r) { return l === r; },
        '!=': function (l, r) { return l != r; },
        '<': function (l, r) { return l < r; },
        '>': function (l, r) { return l > r; },
        '<=': function (l, r) { return l <= r; },
        '>=': function (l, r) { return l >= r; },
        'typeof': function (l, r) { return typeof l == r; }
    }

    if (!operators[operator])
        throw new Error("Handlerbars Helper 'compare' doesn't know the operator " + operator);

    var result = operators[operator](lvalue, rvalue);

    if (result) {
        return options.fn(this);
    } else {
        return options.inverse(this);
    }

});

Handlebars.registerHelper("date", function (date) {
    if (!date || date == undefined) {
        return "";
    }
    return moment(date).format("DD.MM.YYYY");
});

Handlebars.registerHelper("dateFormat", function (date, format) {
    if (!date || date == undefined) {
        return "";
    }
    if (moment.locale() != 'bg')
        moment.locale('bg');
    return moment(date).format(format);
});

Handlebars.registerHelper('compute', function (lvalue, rvalue, options) {

    if (arguments.length < 3)
        throw new Error("Handlerbars Helper 'compute' needs 2 parameters");

    operator = options.hash.operator || "+";

    var operators = {
        '+': function (l, r) { return l + r; },
        '-': function (l, r) { return l - r; },
        '*': function (l, r) { return l * r; },
        '/': function (l, r) { return l / r; }
    }

    if (!operators[operator])
        throw new Error("Handlerbars Helper 'compute' doesn't know the operator " + operator);

    var result = operators[operator](lvalue, rvalue);

    if (result) {
        return options.fn(this);
    } else {
        return options.inverse(this);
    }

});


Handlebars.registerHelper('fixed2', function (value) {
    if (!value || value == undefined) {
        return "";
    }
    return value.toFixed(2);
});