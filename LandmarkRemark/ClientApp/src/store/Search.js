const API_URL = 'api/Notes'
const PREFIX = "SEARCH"
const INITIATE = PREFIX + '_INITIATE';
const SUCCESS = PREFIX + '_SUCCESS';
const ERROR = PREFIX + '_ERROR';
const initialState = {
  searchTerm: '',
  isLoading: false,
  errors: [],
  result: []
};

// DRY up unknow error action
const unknowErrorAction = { type: ERROR, errors: ['Unknow error occured'] }

export const actionCreators = {
  search: searchTerm => async (dispatch, getState) => {
    // don't search for invalid term , term already search
    if (!searchTerm || searchTerm.trim() == "") return;

    try {
      dispatch({ type: INITIATE, searchTerm })
      const url = `${API_URL}?searchTerm=${searchTerm}`
      const response = await fetch(url);
      if (response.status == 200) {
        // can safely return response
        const responseJson = await response.json();
      
        // if request corresponding to current search term, so can discard old results
        if (responseJson.searchTerm == getState().search.searchTerm) {
          // should contain search term and result property
          dispatch({ type: SUCCESS, ...responseJson })
        }

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
