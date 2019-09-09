import React, { Component } from 'react';
//https://dev.to/jessicabetts/how-to-use-google-maps-api-and-react-js-26c2
import { Map, GoogleApiWrapper, Marker, InfoWindow } from 'google-maps-react';
import { connect } from 'react-redux';

import {
  Col, Grid, Row, Form, FormGroup, Alert
  , ControlLabel, Button, FormControl
} from 'react-bootstrap';

import "./css/Home.css"
// getting add note action
import { actionCreators } from "../store/Note"
import { bindActionCreators } from 'redux';

class Home extends Component {
  constructor(props) {
    super(props)
    this.state = {
      geolocationAPIAvailable: false,
      retreiveLocationError: null,
      currentPosition: null,
      showInfoWindow: false,
      activeMarker: null,
      activeMarkerNote: null,
      noteContent: ''
    }
    this.handleInputChange = this.handleInputChange.bind(this)
    this.handleSubmit = this.handleSubmit.bind(this)
  }
  mapStyles = {
    width: '95%',
    height: '95%',
  };
  componentDidMount() {
    if (navigator.geolocation) {

      this.setState({ geolocationAPIAvailable: true });
      navigator.geolocation.getCurrentPosition((position) => {
        const currentPosition = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        };
        this.setState({ currentPosition, retreiveLocationError: undefined }, () => console.log(this.state))
      }, error => {
        this.setState({ retreiveLocationError: error })
      }, { enableHighAccuracy: true }) // get high accuracy location
    };
  }
  // render current location marker
  // pass note into props, but for current location, just 
  // user content = 'Current location' for convenient
  renderCurrentLocationMarker() {

    if (this.state.currentPosition) {
      const iconOptions = {
        url: '/current_location.png',
        scaledSize: new this.props.google.maps.Size(24, 24)
      };
      return (<Marker
        name={'Current location'}
        icon={iconOptions}
        note={{ content: 'Current location' }}
        onClick={this.onMarkerClick.bind(this)}
        position={this.state.currentPosition} />)
    }
  }
  // set active marker and show info window
  onMarkerClick(props, marker, e) {
    this.setState({
      showInfoWindow: true,
      activeMarker: marker,
      activeMarkerNote: props.note
    })

  }

  renderNotesMarker() {
    // don't render when there is no user data
    if (!this.props.userData || !this.props.userData.notes) {
      return "";
    }
    return this.props.userData.notes.map((note, index) => {
      // extract lat long
      const { lat, lng } = note;

      return (
        <Marker
          key={index}
          name={note.content}
          note={note}
          onClick={this.onMarkerClick.bind(this)}
          position={{ lat, lng }} />
      )
    });
  }
  // there is a bug , need to click twice to render infowindow
  renderInfoWindow() {
    if (this.state.showInfoWindow) {

      return (<InfoWindow
        marker={this.state.activeMarker}
        visible={this.state.showInfoWindow}>
        <div>
          <h4>{this.state.activeMarkerNote ? this.state.activeMarkerNote.content : ''}</h4>
        </div>
      </InfoWindow>);
    }

  }
  renderMap() {
    const { geolocationAPIAvailable, retreiveLocationError, currentPosition } = this.state
    if (!geolocationAPIAvailable) {
      return <h2>Geolocation API not available</h2>
    } else if (retreiveLocationError) {
      // there is error retreiving location
      // code ref here https://developer.mozilla.org/en-US/docs/Web/API/PositionError
      switch (retreiveLocationError.code) {
        case 1:
          return <h2>Geolocation permission denied. Please allow the page to retreive location</h2>
        case 2:
          return <h2>Cannot retreive location because of internal source failed</h2>
        case 3:

          return <h2>Timeout trying to get location</h2>
        default:
          return <h2>Unknown error occured</h2>
      }
      // assuming success at this state
    } else {
      // if position = null mean it's getting location, dont load
      if (currentPosition) {
        return (
          <Map
            google={this.props.google}
            style={this.mapStyles}
            initialCenter={currentPosition}
            zoom={15}
          >
            {this.renderCurrentLocationMarker()}
            {this.renderNotesMarker()}
            {this.renderInfoWindow()}
          </Map>
        )

      } else {
        return <h2>Retreiving location...</h2>
      }


    }

  }
  renderAddNoteError() {
    const { errors } = this.props.addNoteState
    console.log("error", errors)
    if (errors && errors.length) {
      return (<Alert bsStyle="danger">
        <ul>
          {errors.map((error, index) => <li key={index}>{error}</li>)}
        </ul>
      </Alert>);
    }
  }
  handleSubmit(event) {
    console.log("")
    event.preventDefault();
    const { currentPosition, noteContent } = this.state;
    this.props.addNote(noteContent, currentPosition.lat, currentPosition.lng)

  }
  handleInputChange(event) {
    this.setState({ noteContent: event.target.value })
  }
  render() {
    return (
      <Grid id="home-container" fluid className="full-height" >
        <Row>
          <Form inline onSubmit={this.handleSubmit}>
            <FormGroup controlId="addingNote">
              <ControlLabel>Adding note: </ControlLabel>
              <FormControl
                type="text"
                value={this.state.noteContent}
                placeholder="e.g: firstUser"
                onChange={this.handleInputChange}
              />
            </FormGroup>
            <FormGroup>
              <Button type="submit"
                disabled={this.props.isLoading}
                bsStyle="primary"
                className="pull-right">Add Note</Button>
            </FormGroup>
            {this.renderAddNoteError()}
          </Form>
        </Row>
        <Row>
          {this.renderMap()}
        </Row>

      </Grid>
    );
  }
}



export default connect(
  state => {
    return {
      ...state.login,
      addNoteState :state.note
    }
  },
  dispatch => bindActionCreators(actionCreators, dispatch)
)(GoogleApiWrapper({
  apiKey: 'AIzaSyDoIS4mmofwgjU28TC1WUmpNU3zuHtXPWM'
})(Home));
