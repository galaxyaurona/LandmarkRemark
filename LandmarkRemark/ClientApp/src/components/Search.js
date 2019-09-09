import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/Search';
import {
  Form, FormGroup, Alert
  , ControlLabel, Button, FormControl
} from 'react-bootstrap';

class Search extends Component {
  constructor(props) {
    super(props)
    this.state = {
      searchTerm: ''
    }
    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }
  handleSubmit(event) {
    this.props.search(this.state.searchTerm);
    event.preventDefault();
  }
  handleInputChange(event) {
    this.setState({ searchTerm: event.target.value });
  }
  renderSearchError() {
    const { errors } = this.props
    if (errors && errors.length) {
      <Alert bsStyle="danger">
        <ul>
          {errors.map((error, index) => <li key={index}>{error}</li>)}
        </ul>
      </Alert>;
    }
  }
  renderSearchResult() {
    const { result } = this.props;
 
    if (result && result.length) {
    
      // render result
      return (<ol>

        { 
          result.map( (item, index) => {
            return <li key={index}>Note content: '{item.content}' - by user: {item.owner.username}</li>
          })
        }
      </ol>)
    }
  }
  render() {
    return (
      <div>
        <Form inline onSubmit={this.handleSubmit}>
          <FormGroup controlId="addingNote">
            <ControlLabel>Search for by username or content </ControlLabel>
            <FormControl
              type="text"
              value={this.state.searchTerm}
              placeholder="e.g: firstUser"
              onChange={this.handleInputChange}
            />
          </FormGroup>
          <FormGroup>
            <Button type="submit"
              disabled={this.props.isLoading}
              bsStyle="primary"
              className="pull-right"
              disable={this.isLoading}
            >Search</Button>
          </FormGroup>
        </Form>
        {this.renderSearchError()}
        {this.renderSearchResult()}
      </div>
    );
  }
}


export default connect(
  state => state.search,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(Search);
