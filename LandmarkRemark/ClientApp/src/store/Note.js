const API_URL = 'api/Notes'
const PREFIX = "ADD_NOTE"
const INITIATE = PREFIX + '_INITIATE';
const SUCCESS = PREFIX + '_SUCCESS';
const ERROR = PREFIX + '_ERROR';

const initialState = {
  noteContent: '',
  isLoading: false,
  errors: [],

};

const unknowErrorAction = { type: ERROR, errors: ['Unknow error occured'] }


export const actionCreators = {
  addNote: (content, lat, lng) => async (dispatch, getState) => {

    const loginState = getState().login;
    // compose data to add, with user id from global state
    const data = {
      content,
      lat,
      lng,
      UserId: loginState.userData.id,
    }
    const errors = []
    console.log("data", data, loginState)
    // dont add stuff when not logged in
    if (!loginState.isAuthenticated) {
      errors.push('You must login to add note')
      
    }
    if (!content || content.trim() == "") {
      errors.push('Content cannot be empty');
    
    }
    if (errors.length > 0) {
      dispatch({ type: ERROR, errors })
      return
    }

    try {
      dispatch({ type: INITIATE })
      const url = `${API_URL}`


      const fetchOptions = {
        method: 'POST',
        body: JSON.stringify(data),
        headers: {
          'Content-Type': 'application/json'
        }
      }
      const response = await fetch(url, fetchOptions);
      if (response.status == 201) {
        // can safely return response
        const responseJson = await response.json();

        // if request corresponding to current search term, so can discard old results
        // should contain search term and result property
        dispatch({ type: SUCCESS, ...responseJson })


      }
      else {
        dispatch(unknowErrorAction)
      }
    } catch (errors) {
      dispatch(unknowErrorAction)
    }

  }
};

export const reducer = (state = initialState, action) => {
  switch (action.type) {
    case INITIATE:
      return {
        ...state,
        searchTerm: action.searchTerm,

        isLoading: true,
        errors: [],
        result: [],
      }
    case ERROR:
      console.log("error", action)
      return {
        ...state,
        isLoading: false,
        errors: action.errors
      };
    case SUCCESS:

      return { ...state, isLoading: false, result: action.result }
    default:
      return state;
  }
  return state;
};