import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import { connect } from "react-redux"
import './css/NavMenu.css';


// issue with nav link causing it to render twice
class NavMenu extends Component {
  render() {
    return (
      <Navbar inverse fixedTop fluid collapseOnSelect>
        <Navbar.Header>
          <Navbar.Brand>
            <Link to={'/'}>LandmarkRemark</Link>
          </Navbar.Brand>
          <Navbar.Toggle />
        </Navbar.Header>
        {this.props.isAuthenticated ?
          <Navbar.Collapse>
            <Nav>
              <LinkContainer to={'/'} exact>
                <NavItem>
                  <Glyphicon glyph='home' /> Home
              </NavItem>
              </LinkContainer>
              <LinkContainer to={'/search'} >
                <NavItem>
                  <Glyphicon glyph='search' /> Search
              </NavItem>
              </LinkContainer>
            </Nav>
          </Navbar.Collapse>
          : <div></div>
        }
      </Navbar>
    );
  }
}


export default connect(state => state.login)(NavMenu)