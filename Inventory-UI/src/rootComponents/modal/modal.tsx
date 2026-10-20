export default function Modal({ text, children } : { text: string, children?: React.ReactNode }) {
    return (
        <>
            <button className="btn" onClick={()=> document.getElementById('my_modal_2').showModal()}>open modal</button>
            <div>
                <dialog id="my_modal_2" className="modal">
                    <div className="modal-box">
                        {children}
                    </div>
                    <form method="dialog" className="modal-backdrop">
                        <button>{text}</button>
                    </form>
                </dialog>
            </div>
        </>
    )
}