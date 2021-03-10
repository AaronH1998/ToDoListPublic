function CreateBody(props){
    return(
        <div className="modal-body">
            <label name="bodyTitle">{props.object} Name:</label>
            <input style={{ display: "inline" }} id="input" type="text" />
            <button id="submitModalButton" style={{ display: "inline" }, { float: "right" }} type="button" data-dismiss="modal" className="btn btn-black">
                {props.type}
            </button>
        </div>
    );
}

function DeleteBody(props) {
    return (
        <div className="modal-body">
            <label name="bodyTitle">Are you sure you want to delete this {props.object} </label>
            <button id="submitModalButton" style={{ display: "inline" }, { float: "right" }} type="button" data-dismiss="modal" className="btn btn-black btn-sm">
                {props.type}
            </button>
        </div>
    );
}


function ModalBody(props) {
    let modalBody;
    if (props.type == "Create" || "Edit") {
        modalBody = <CreateBody object={props.object} type={props.type} />
    } else if (props.type == "Delete" || "Complete") {
        modalBody = <DeleteBody object={props.object} type={props.type} />
    }

    return (
        <div>
            {modalBody}
        </div>
    );
}

function Modal(props) {
    return (
        <div className="modal-dialog">
            <div className="modal-content">
                <div className="modal-header">
                    <h5 className="modal-title" id="headerLabel">{props.type} {props.object}</h5>
                    <button type="button" className="close" data-dismiss="modal" aria-label="close">
                        <span aria-hidden="true">x</span>
                    </button>
                </div>
                <ModalBody object={props.object} type={props.type} />
            </div>
        </div>
    );
}

const allModals = $(".react-modal");

allModals.each((index,element) => {
    ReactDOM.render(
        <Modal object={element.getAttribute("object")} type={element.getAttribute("type")} />,
        element
    );

    const onClickFunction = element.getAttribute("onCompletion");

    $(".modal .modal-body #submitModalButton").click((event) => {
        var inputValue = $(event.target).prev("input").val();
        if (inputValue) {
            window[onClickFunction](inputValue);
        } else {
            window[onClickFunction];
        }
    });
});