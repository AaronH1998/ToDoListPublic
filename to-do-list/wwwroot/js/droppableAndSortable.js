var dropped = false;

$(function () {

    setSortable();

    $(".droppable").droppable({
        activeClass: 'active',
        tolerance: 'pointer',
        hoverClass: 'hovered',
        accept: ".drop",
        drop: function (event,ui) {
            dropped = true;
            var taskId = ui.draggable.attr('id');
            var targetList = $(this).text().trim().toString();

            $.ajax({
                url: "/api/tasks/" + taskId,
                method: "put",
                contentType: "application/json",
                data: JSON.stringify({ content: targetList }),
                success: async function(result) {
                    await tasksVM.load();
                    await updatePriority(event,ui);
                    tasksVM.load();
                }
            });
        }
    });

    updatePriority = function (event,ui) {
        if (dropped) {
            var i = 1;
            for (var task of tasksVM.tasks()) {
                task.priority = i;
                i++;
            }
            return $.ajax({
                url: "/api/tasks/sortable",
                method: "put",
                dataType: "application/json",
                data: { tasks: tasksVM.tasks() }
            });
        } else {
            $("tbody > tr").each(function (i) {
                var id = $(this).attr('id');
                var task = tasksVM.tasks().find(p => p.taskId == id);
                task.priority = i + 1;
            });

            if (tasksVM.listType() != 'Completed') {
                return $.ajax({
                    url: "/api/tasks/sortable",
                    method: "put",
                    dataType: "application/json",
                    data: { tasks: tasksVM.tasks() }
                });
            }
        }
        
    };
});

function setSortable() {
    $('#sortable').sortable({
        stop: async function (event,ui) {
            if (dropped) {
                dropped = false;
                setSortable();
            } else {
                await updatePriority(event,ui);
                if (tasksVM.listType() != 'Completed') {
                    tasksVM.load();
                }
            }
        }
    });
}