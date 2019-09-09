import React, { Component } from "react";
import {
    Form, FormGroup,
    ControlLabel, FormControl,
    Alert, Button,
} from "react-bootstrap";
import Panel from "react-bootstrap/lib/Panel"
import "./css/Login.css"
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from "../store/Login"
import { push } from 'react-router-redux'
class Login extends Component {

    constructor(props) {
        super(props);

        // state only have username
        this.state = {
            username: ''
        }
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }
    //handle username change
    handleChange(event) {
        this.setState({ username: event.target.value });
    }
    // validate username before sending request
    isValidUsername(username) {
        return username && username.trim() != "";
    }
    handleSubmit(event) {
        // handle form submit, i.e submit button
        let { username } = this.state;
        event.preventDefault();
        if (this.isValidUsername(username)) {
            this.props.login(username)
        }
    }
    // redirect once login success to home page
    componentWillUpdate(nextProps) {
        if (nextProps.isAuthenticated) {
            this.props.history.push('/');
        }
    }
    // render error alert from list of errors
    renderErrorAlert(errors) {
        if (errors.length) {
            return <Alert bsStyle="danger">
                <ul>
                    {errors.map((error, index) => <li key={index}>{error}</li>)}
                </ul>
            </Alert>;

        }
        // no error
        return null;
    }
    render() {

        return (

            <div className="full-height flex flex-center" >
                <Panel bsStyle="primary">
                    <Panel.Heading>Login</Panel.Heading>
                    <Panel.Body>
                        <Form horizontal onSubmit={this.handleSubmit.bind(this)}>
                            <FormGroup controlId="username">
                                <ControlLabel>User name</ControlLabel>
                                <FormControl
                                    type="text"
                                    value={this.state.username}
                                    placeholder="e.g: firstUser"
                                    onChange={this.handleChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <Button type="submit"
                                    disabled={this.props.isLoading}
                                    bsStyle="primary"
                                    className="pull-right"
                                    onClick={this.tryLogin}>Sign in</Button>
                            </FormGroup>
                        </Form>
                        {this.renderErrorAlert(this.props.errors)}
                    </Panel.Body>

                </Panel>
            </div>

        );
    }
}

// bind global state to props
export default connect(state => state.login,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Login)