import React, { Component } from "react"
import { connect } from "react-redux"

// a HOC to compose a component that check for auth and redirect
export default function (ComposedComponent) {
    class RequireAuth extends Component {
        componentWillMount() {
            // not authenticated
            if (!this.props.isAuthenticated) {
                this.props.history.push("/login")
            } 
        }
        componentWillUpdate(nextProps) {

            if (!nextProps.auth.authenticated) {
                this.props.history.push("/login")
            }
        }
        render() {
            return <ComposedComponent {...this.props} />
        }
    }
    return connect(state => state.login
        
        )(RequireAuth)
}