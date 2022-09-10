const Error = (props) => {
    return (
        <div className="alert alert-danger text-center" role="alert">
            {props.message}
        </div>
    );
}
export default Error
