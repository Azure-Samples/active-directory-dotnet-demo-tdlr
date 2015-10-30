function showError() {
    $(".generic-error").show("slow");
}

$("select.task-status").change(function (e) {
    $select = $(this);
    ogVal = $select.attr('data-og-value');
    var $optionSelected = $select.children("option:selected");
    if ($optionSelected.val() != ogVal) {
        $tr = $select.parents("tr");
        $tr.addClass("warning");
        $tr.find(".task-save").show("slow");
        $tr.find(".task-delete").hide("slow");
    } else {
        $tr = $select.parents("tr");
        $tr.removeClass("warning");
        $tr.find(".task-save").hide("slow");
        $tr.find(".task-delete").show("slow");
    }
});

$("button.task-delete").click(function (e) {
    $(this).siblings("div.task-confirm").children().show();
});

$("button.task-cancel").click(function (e) {
    $(this).parent().hide();
});

function appendShare(obj) {
    $tbody = $(".modal-body").find("tbody");
    $tbody.append('<tr class="share-row" data-objectId="' + obj.objectId + '" data-displayName="' + obj.displayName + '"><td>' + obj.displayName + '</td><td><button name="remove" type="button" value="' + obj.objectId + '" class="share-action share-remove" onclick="removeShare(this)"><i class="fa fa-times fa-lg"></i></button></td></tr>')
}

$("button.task-share").click(function (e) {
    $share = $(this)
    $(".modal-title").text($share.parent().siblings(".task-text").text())
    $(".modal-content").attr("data-taskId", $share.attr("value"));
    $.getJSON("/api/tasks/" + $share.attr("value") + "/share", function (json) {
        json.map(appendShare);
    });
});

$("button.task-save").click(function (e) {
    var taskId = $(this).val();
    var $status = $(this).parents("tr").find("select.task-status");
    var newStatus = $status.val();
    $.ajax({
        url: "/api/tasks/" + taskId,
        type: "PUT",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify({
            Status: newStatus,
        }),
        beforeSend: function (jqxhr, settings) {
            jqxhr.overrideMimeType("application/json");
        }
    })
    .done(function (task) {
        $status.attr("data-og-value", newStatus);
        $status.trigger("change");
    })
    .fail(showError);
});

$("button.task-confirm").click(function (e) {
    var taskId = $(this).val();
    var $btn = $(this);
    $.ajax({
        url: "/api/tasks/" + taskId,
        type: "DELETE",
    })
    .done(function (task) {
        $tr = $btn.parents("tr");
        $tr.hide("slow", function () { $tr.remove(); });
    })
    .fail(showError);
});

$("button.btn-share").click(function (e) {
    var taskId = $(this).parents(".modal-content").attr("data-taskId");
    $shares = $(this).parents(".modal-content").find(".share-row")
    var shares = []
    $shares.each(function () {
        shares.push({
            objectId: $(this).attr("data-objectId"),
            displayName: $(this).attr("data-displayName")
        });
        $(this).remove();
    });

    $.ajax({
        url: "/api/tasks/" + taskId + "/share",
        type: "PUT",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(shares),
        beforeSend: function (jqxhr, settings) {
            jqxhr.overrideMimeType("application/json");
        }
    })
    .fail(showError);
});


$("button.btn-share-cancel").click(function (e) {
    $(this).parents(".modal-content").find("tbody").empty();
});

function removeShare(dom) {
    $tr = $(dom).parents("tr");
    $tr.hide("slow");
    $tr.remove();
};